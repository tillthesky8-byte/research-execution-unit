using Contracts.Rows;

namespace Contracts.Models;

public class Position
{
    public required string Symbol { get; init; }
    public required decimal Quantity { get; set; }
    public bool IsLong => Quantity > 0;
    public bool IsShort => Quantity < 0;
    public decimal AverageEntryPrice { get; set; }
    public decimal GetUnrealizedPnl(MarketContext marketContext)
    {
        decimal currentPrice = marketContext.PriceData[Symbol].Close;
        decimal pnlPerUnit = IsLong ? currentPrice - AverageEntryPrice : AverageEntryPrice - currentPrice;
        return pnlPerUnit * Math.Abs(Quantity);
    }
    public decimal GetMarketValue(MarketContext marketContext)
    {        
        decimal currentPrice = marketContext.PriceData[Symbol].Close;
        return currentPrice * Quantity;
    }
}