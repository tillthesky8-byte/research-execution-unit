using Contracts.Enums;
using Contracts.Interfaces;
using Contracts.Rows;
using Microsoft.Extensions.Logging;

namespace Contracts.Models;
public sealed class Portfolio(decimal initialCash, ILogger<Portfolio>? logger) : IReadOnlyPortfolio
{
    private readonly ILogger<Portfolio>?          _logger       = logger     ;
    private readonly Dictionary<string, Position> _positions    = new()      ;
    private          List<Trade>                  _tradeHistory = new()      ;
    private          decimal                      _cash         = initialCash;

    public           IReadOnlyDictionary<string, Position> Positions    => _positions   ;
    public           IReadOnlyList<Trade>                  TradeHistory => _tradeHistory;
    public           decimal                               Cash         => _cash        ;

    public decimal GetEquity(MarketRow MarketRow)
    {
        decimal positionValue = _positions.Values.Sum(p => p.GetMarketValue(MarketRow));
        return positionValue + _cash;
    }
    public void UpdateCash(decimal amount) => _cash += amount;  

    public TradeAction AdjustPosition(string symbol, decimal quantityDelta, decimal price, DateTime timestamp)
    {
        if (quantityDelta == 0) throw new ArgumentException("Quantity delta cannot be zero. Review the strategy logic.", nameof(quantityDelta));
       
        _positions.TryGetValue(symbol, out Position? existingPosition);
        
        if (existingPosition is null) return OpenPosition(symbol, quantityDelta, price, timestamp);

        decimal newQuantity;
        try
        {
            newQuantity = checked(existingPosition.Quantity + quantityDelta);
        }
        catch (OverflowException)
        {
            _logger?.LogError("Overflow detected in AdjustPosition: existing quantity {ExistingQuantity} + delta {QuantityDelta} exceeds decimal limits for symbol {Symbol}", existingPosition.Quantity, quantityDelta, symbol);
            throw;
        }

        // Validate that position doesn't exceed reasonable limits
        const decimal MAX_POSITION_SIZE = 1_000_000_000m; // 1 billion units
        if (Math.Abs(newQuantity) > MAX_POSITION_SIZE)
        {
            _logger?.LogError("Position size validation failed: calculated quantity {NewQuantity} exceeds maximum of {MaxSize} for symbol {Symbol}", newQuantity, MAX_POSITION_SIZE, symbol);
            throw new ArgumentException($"Position size exceeds maximum allowed value of {MAX_POSITION_SIZE} units", nameof(newQuantity));
        }
    
        if         (IsReversal(existingPosition.Quantity, newQuantity))

            return ReversePosition(symbol, quantityDelta, price, timestamp);

        else if    (IsScaleIn(existingPosition.Quantity, quantityDelta))

            return ScaleInPosition(symbol, quantityDelta, price, timestamp);
            
        else if    (IsPartialClose(existingPosition.Quantity, quantityDelta))

            return PartialClosePosition(symbol, quantityDelta, price, timestamp);
        
        else if    (isFullClose(existingPosition.Quantity, quantityDelta))
            
            return PartialClosePosition(symbol, quantityDelta, price, timestamp);
        else           
         
         throw new InvalidOperationException($"Unexpected position adjustment scenario for symbol {symbol} with existing quantity {existingPosition.Quantity}");
    }

    private bool IsReversal(decimal oldQuantity, decimal newQuantity) =>
        Math.Sign(oldQuantity) != Math.Sign(newQuantity) && oldQuantity != 0 && newQuantity != 0;

    private bool IsScaleIn(decimal existingQuantity, decimal quantityDelta) =>
        Math.Sign(existingQuantity) == Math.Sign(quantityDelta) && existingQuantity != 0 && quantityDelta != 0;
    private bool isFullClose(decimal existingQuantity, decimal quantityDelta) =>
        existingQuantity + quantityDelta == 0 && existingQuantity != 0 && quantityDelta != 0;
    private bool IsPartialClose(decimal existingQuantity, decimal quantityDelta) =>
        Math.Sign(existingQuantity) != Math.Sign(quantityDelta) && existingQuantity + quantityDelta != 0 && existingQuantity != 0 && quantityDelta != 0;
    
    private TradeAction OpenPosition(string symbol, decimal quantity, decimal price, DateTime timestamp)
    {
        var trade = new Trade(
            Symbol: symbol,
            Timestamp: timestamp,
            Side: quantity > 0 ? OrderSide.Buy : OrderSide.Sell,
            Quantity: Math.Abs(quantity),
            Price: price,
            CommissionPaid: 0,
            Action: TradeAction.Open
        );

        _tradeHistory.Add(trade);
        _positions[symbol] = new Position { Symbol = symbol, Quantity = quantity, AverageEntryPrice = price };
        return TradeAction.Open;    
    }

    private TradeAction ReversePosition(string symbol, decimal quantityDelta, decimal price, DateTime timestamp)
    {
        var trade = new Trade(
            Symbol:    symbol,
            Timestamp: timestamp,
            Side:      quantityDelta > 0 ? OrderSide.Buy : OrderSide.Sell,
            Quantity:  Math.Abs(quantityDelta),
            Price:     price,
            CommissionPaid: 0,
            Action:    TradeAction.Reverse
        );

        _tradeHistory.Add(trade);

        var existingPosition = _positions[symbol];
        existingPosition.Quantity += quantityDelta;
        existingPosition.AverageEntryPrice = price;
        return TradeAction.Reverse;    
    }
    
    private TradeAction ScaleInPosition(string symbol, decimal quantityDelta, decimal price, DateTime timestamp)
    {
        try
        {
            var trade = new Trade(
                Symbol:         symbol,
                Timestamp:      timestamp,
                Side:           quantityDelta > 0 ? OrderSide.Buy : OrderSide.Sell,
                Quantity:       Math.Abs(quantityDelta),
                Price:          price,
                CommissionPaid: 0,
                Action:         TradeAction.ScaleIn
            );

            _tradeHistory.Add(trade);

            var existingPosition = _positions[symbol];
            decimal existingAbsQuantity = Math.Abs(existingPosition.Quantity);
            decimal deltaAbsQuantity = Math.Abs(quantityDelta);
            decimal newAbsQuantity = checked(existingAbsQuantity + deltaAbsQuantity);
            
            existingPosition.AverageEntryPrice = checked(
                (existingPosition.AverageEntryPrice * existingAbsQuantity + price * deltaAbsQuantity) / newAbsQuantity
            );
            existingPosition.Quantity         += quantityDelta;
            return TradeAction.ScaleIn;   
        }
        catch (OverflowException ex)
        {
            _logger?.LogError(ex, "Overflow in ScaleInPosition for symbol {Symbol} with quantity delta {QuantityDelta} at price {Price}. Existing quantity: {ExistingQuantity}, existing avg price: {AvgPrice}", symbol, quantityDelta, price, _positions[symbol].Quantity, _positions[symbol].AverageEntryPrice);
            throw;
        }
    }

    private TradeAction PartialClosePosition(string symbol, decimal quantityDelta, decimal price, DateTime timestamp)
    {
        var trade = new Trade(
            Symbol:         symbol,
            Timestamp:      timestamp,
            Side:           quantityDelta > 0 ? OrderSide.Buy : OrderSide.Sell,
            Quantity:       Math.Abs(quantityDelta),
            Price:          price,
            CommissionPaid: 0,
            Action:         TradeAction.PartialClose
        );

        _tradeHistory.Add(trade);

        var existingPosition       = _positions[symbol];
        existingPosition.Quantity += quantityDelta;
        
        if (existingPosition.Quantity == 0)
        {
            _positions.Remove(symbol);
            return TradeAction.Close;
        }             
        return TradeAction.PartialClose;    
    }
}