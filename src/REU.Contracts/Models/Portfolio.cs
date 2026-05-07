using Contracts.Enums;
using Contracts.Interfaces;
using Contracts.Rows;

namespace Contracts.Models;
public sealed class Portfolio(decimal initialCash) : IReadOnlyPortfolio
{
    private readonly Dictionary<string, Position> _positions = new();
    private List<Trade> _tradeHistory = new();
    private decimal _cash = initialCash;

    public IReadOnlyDictionary<string, Position> Positions => _positions;
    public IReadOnlyList<Trade> TradeHistory => _tradeHistory;
    public decimal Cash => _cash;

    public decimal GetEquity(MarketContext marketContext)
    {
        decimal positionValue = _positions.Values.Sum(p => p.GetMarketValue(marketContext));
        return positionValue + _cash;
    }
    public void UpdateCash(decimal amount) => _cash += amount;  

    public TradeAction AdjustPosition(string symbol, decimal quantityDelta, decimal price, DateTime timestamp)
    {
        _positions.TryGetValue(symbol, out Position? existingPosition);
        
        if (existingPosition is null) return OpenPosition(symbol, quantityDelta, price, timestamp);

        decimal newQuantity = existingPosition.Quantity + quantityDelta;

        if (IsReversal(existingPosition.Quantity, newQuantity))
            return ReversePosition(symbol, quantityDelta, price, timestamp);

        else if (IsScaleIn(existingPosition.Quantity, quantityDelta))
            return ScaleInPosition(symbol, quantityDelta, price, timestamp);
            
        else
            return PartialClosePosition(symbol, quantityDelta, price, timestamp);
    }

    private bool IsReversal(decimal oldQuantity, decimal newQuantity) =>
        Math.Sign(oldQuantity) != Math.Sign(newQuantity) && oldQuantity != 0 && newQuantity != 0;

    private bool IsScaleIn(decimal existingQuantity, decimal quantityDelta) =>
        Math.Sign(existingQuantity) == Math.Sign(quantityDelta);
    
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
        var trade = new Trade(
            Symbol:    symbol,
            Timestamp: timestamp,
            Side:      quantityDelta > 0 ? OrderSide.Buy : OrderSide.Sell,
            Quantity:  Math.Abs(quantityDelta),
            Price:     price,
            CommissionPaid: 0,
            Action:    TradeAction.ScaleIn
        );

        _tradeHistory.Add(trade);

        var existingPosition = _positions[symbol];
        existingPosition.AverageEntryPrice = (existingPosition.AverageEntryPrice * Math.Abs(existingPosition.Quantity) + price * Math.Abs(quantityDelta)) / Math.Abs(existingPosition.Quantity + quantityDelta);
        existingPosition.Quantity += quantityDelta;
        return TradeAction.ScaleIn;    
    }

    private TradeAction PartialClosePosition(string symbol, decimal quantityDelta, decimal price, DateTime timestamp)
    {
        var trade = new Trade(
            Symbol:    symbol,
            Timestamp: timestamp,
            Side:      quantityDelta > 0 ? OrderSide.Buy : OrderSide.Sell,
            Quantity:  Math.Abs(quantityDelta),
            Price:     price,
            CommissionPaid: 0,
            Action:    TradeAction.PartialClose
        );

        _tradeHistory.Add(trade);

        var existingPosition = _positions[symbol];
        existingPosition.Quantity += quantityDelta;
        
        if (existingPosition.Quantity == 0)
        {
            _positions.Remove(symbol);
            return TradeAction.Close;
        }             
        return TradeAction.PartialClose;    
    }
}