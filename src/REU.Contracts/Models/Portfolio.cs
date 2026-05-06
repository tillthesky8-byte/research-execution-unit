using Contracts.Enums;
using Contracts.Interfaces;
using Contracts.Rows;

namespace Contracts.Models;
public sealed class Portfolio(decimal initialCash) : IReadOnlyPortfolio
{
    private readonly Dictionary<string, Position> _positions = new();
    private decimal _cash = initialCash;

    public IReadOnlyDictionary<string, Position> Positions => _positions;
    public decimal Cash => _cash;

    public decimal GetEquity(MarketContext marketContext)
    {
        decimal positionValue = _positions.Values.Sum(p => p.GetMarketValue(marketContext));
        return positionValue + _cash;
    }

    public TradeAction AdjustPosition(string symbol, decimal quantityDelta, decimal price)
    {
        _positions.TryGetValue(symbol, out Position? existingPosition);
        
        if (existingPosition is null) return OpenPosition(symbol, quantityDelta, price);

        decimal newQuantity = existingPosition.Quantity + quantityDelta;

        if (IsReversal(existingPosition.Quantity, newQuantity))
            return ReversePosition(symbol, quantityDelta, price);

        else if (IsScaleIn(existingPosition.Quantity, quantityDelta))
            return ScaleInPosition(symbol, quantityDelta, price);
            
        else
            return PartialClosePosition(symbol, quantityDelta, price);
    }

    private bool IsReversal(decimal oldQuantity, decimal newQuantity) =>
        Math.Sign(oldQuantity) != Math.Sign(newQuantity) && oldQuantity != 0 && newQuantity != 0;

    private bool IsScaleIn(decimal existingQuantity, decimal quantityDelta) =>
        Math.Sign(existingQuantity) == Math.Sign(quantityDelta);
    
    private TradeAction OpenPosition(string symbol, decimal quantity, decimal price)
    {
        _positions[symbol] = new Position { Symbol = symbol, Quantity = quantity, AverageEntryPrice = price };
        return TradeAction.Open;    
    }

    private TradeAction ReversePosition(string symbol, decimal quantityDelta, decimal price)
    {
        var existingPosition = _positions[symbol];
        _positions[symbol] = new Position
        {
            Symbol = symbol,
            Quantity = existingPosition.Quantity + quantityDelta,
            AverageEntryPrice = price
        };
        return TradeAction.Reverse;    
    }
    
    private TradeAction ScaleInPosition(string symbol, decimal quantityDelta, decimal price)
    {
        var existingPosition = _positions[symbol];
        existingPosition.Quantity += quantityDelta;
        existingPosition.AverageEntryPrice = ((existingPosition.AverageEntryPrice * (existingPosition.Quantity - quantityDelta)) + (quantityDelta * price)) / existingPosition.Quantity;
        return TradeAction.ScaleIn;    
    }

    private TradeAction PartialClosePosition(string symbol, decimal quantityDelta, decimal price)
    {
        var existingPosition = _positions[symbol];
        existingPosition.Quantity += quantityDelta;
        return TradeAction.PartialClose;    
    }
}