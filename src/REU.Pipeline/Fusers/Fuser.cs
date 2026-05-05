using Contracts.Interfaces;
using Contracts.Rows;
using Contracts.Models;
using Microsoft.Extensions.Logging;

namespace Pipeline.Fusers;

public class FuserLOCF : IFueser
{
    private readonly ILogger<FuserLOCF> _logger;
    public FuserLOCF(ILogger<FuserLOCF> logger)
    {
        _logger = logger;
    }
    public IReadOnlyList<MarketContext> Fuse(IReadOnlyDictionary<string, IReadOnlyList<OhlcvRow>> ohlcvData, IReadOnlyDictionary<string, IReadOnlyList<FactorRow>> factorData)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var marketContexts = InitializeMarketContexts(ohlcvData);
        stopwatch.Stop();
        _logger.LogDebug("Initialized {Count} market contexts from OHLCV data in {ElapsedMilliseconds} ms", marketContexts.Count, stopwatch.ElapsedMilliseconds);

        stopwatch.Restart();
        var factorCursors = factorData.ToDictionary
        (
                kvp => kvp.Key,
                kvp => new Cursor { Index = 0, LastObservedValue = null },
                StringComparer.OrdinalIgnoreCase
        );

        foreach (var context in marketContexts)
        {
            foreach (var (factorName, factorRows) in factorData)
            {
                var cursor = factorCursors[factorName];
                while (cursor.Index < factorRows.Count && factorRows[cursor.Index].Timestamp <= context.Timestamp)
                {
                    cursor.LastObservedValue = factorRows[cursor.Index].Value;
                    cursor.Index++;
                }
                context.FactorData[factorName] = cursor.LastObservedValue;
            }
        }
        stopwatch.Stop();
        _logger.LogDebug("Fused factor data into market contexts using LOCF in {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        return marketContexts;

    }

    private List<MarketContext> InitializeMarketContexts(IReadOnlyDictionary<string, IReadOnlyList<OhlcvRow>> ohlcvData)
    {
        var marketContexts = new List<MarketContext>();
        foreach (var (symbol, series) in ohlcvData)
        {
            foreach (var row in series)
            {
                var bar = new OhlcvBar
                {
                    Open = row.Open,
                    High = row.High,
                    Low = row.Low,
                    Close = row.Close,
                    Volume = row.Volume
                };

                var context = new MarketContext
                {
                    Timestamp = row.Timestamp,
                    PriceData = new Dictionary<string, OhlcvBar>(StringComparer.OrdinalIgnoreCase)
                    {
                        [symbol] = bar
                    }
                };
                marketContexts.Add(context);
            }
        }
        return marketContexts;
    }
}