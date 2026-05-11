using Contracts.Rows;

namespace Contracts.Models;

public class Market(IReadOnlyList<MarketRow> rows)
{
    public List<MarketRow> Rows { get; } = rows.ToList();
    public Dictionary<string, IEnumerable<OhlcvCandle>> ToOhlcvSeries()
    {
        var ohlcBySymbol = new Dictionary<string, List<OhlcvCandle>>(StringComparer.OrdinalIgnoreCase);
        foreach (var marketRow in Rows)
        {
            foreach (var kvp in marketRow.PriceData)
            {
                var symbol = kvp.Key;
                var ohlcvBar = kvp.Value;
                if (!ohlcBySymbol.TryGetValue(symbol, out var ohlcvList))
                {
                    ohlcvList = new List<OhlcvCandle>();
                    ohlcBySymbol[symbol] = ohlcvList;
                }
                ohlcvList.Add(new OhlcvCandle
                {
                    Time = ToUnixTimestamp(marketRow.Timestamp),
                    Open = ohlcvBar.Open,
                    High = ohlcvBar.High,
                    Low = ohlcvBar.Low,
                    Close = ohlcvBar.Close
                });
            }
        }
        return ohlcBySymbol.ToDictionary(kvp => kvp.Key, kvp => (IEnumerable<OhlcvCandle>)kvp.Value);
    }

    private long ToUnixTimestamp(DateTime dateTime) => new DateTimeOffset(dateTime).ToUnixTimeSeconds();
    
}