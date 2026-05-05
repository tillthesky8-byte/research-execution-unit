namespace Contracts.Rows;

public record MarketContext
{
    public DateTime Timestamp { get; init; }
    public Dictionary<string, OhlcvRow> PriceData { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, decimal> FactorData { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    public OhlcvRow this[string symbol] => PriceData.TryGetValue(symbol, out var ohlcv) ? ohlcv : throw new KeyNotFoundException($"Price data for symbol '{symbol}' not found in market context.");

}