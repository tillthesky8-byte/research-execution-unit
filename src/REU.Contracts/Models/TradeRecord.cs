namespace Contracts.Models;

public record TradeRecord
{
    public string Symbol { get; init; }
    public long Time { get; init; }
    public string Side { get; init; }
    public decimal Quantity { get; init; }
    public decimal Price { get; init; }
    public decimal CommissionPaid { get; init; }
    public string Action { get; init; }

    public TradeRecord(Trade trade)
    {
        Symbol = trade.Symbol;
        Time = toUnixTimestamp(trade.Timestamp);
        Side = trade.Side.ToString();
        Quantity = trade.Quantity;
        Price = trade.Price;
        CommissionPaid = trade.CommissionPaid;
        Action = trade.Action.ToString();
    }
    private long toUnixTimestamp(DateTime dateTime) => new DateTimeOffset(dateTime).ToUnixTimeSeconds();
}