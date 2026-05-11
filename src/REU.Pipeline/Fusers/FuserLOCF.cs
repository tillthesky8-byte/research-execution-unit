using Contracts.Interfaces;
using Contracts.Rows;
using Contracts.Models;
using Microsoft.Extensions.Logging;

namespace Pipeline.Fusers;

public class FuserLOCF : IFuser
{
    private readonly ILogger<FuserLOCF>? _logger;
    public  FuserLOCF(ILogger<FuserLOCF> logger)
    {
        _logger = logger;
    }
    public IReadOnlyList<MarketRow> Fuse(IReadOnlyDictionary<string, IReadOnlyList<OhlcvRow>> ohlcvData, IReadOnlyDictionary<string, IReadOnlyList<FactorRow>> factorData)
    {
        var stopwatch     = System.Diagnostics.Stopwatch.StartNew();

        var MarketRows    = InitializeMarketRows(ohlcvData);

        var factorCursors = factorData.ToDictionary
        (
                kvp => kvp.Key,
                kvp => new Cursor { Index = 0, LastObservedValue = null },
                StringComparer.OrdinalIgnoreCase
        );

        foreach (var context in MarketRows)
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
        _logger?.LogDebug("Fused factor data into market contexts using LOCF in {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        return MarketRows;

    }

    private List<MarketRow> InitializeMarketRows(IReadOnlyDictionary<string, IReadOnlyList<OhlcvRow>> ohlcvData)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var timeline = ohlcvData
            .SelectMany(kvp => kvp.Value.Select(row => row.Timestamp))
            .OrderBy(t => t)
            .Distinct()
            .ToList();
        
        var index = ohlcvData.ToDictionary
        (
            kvp => kvp.Key,
            kvp => kvp.Value.ToDictionary
            (
                row => row.Timestamp,
                row => new OhlcvBar
                {
                    Open   = row.Open,
                    High   = row.High,
                    Low    = row.Low,
                    Close  = row.Close,
                    Volume = row.Volume
                }
            ),
            StringComparer.OrdinalIgnoreCase
        );
        var carry = new MarketCarryState();
        var MarketRows = new List<MarketRow>();

        foreach (var timestamp in timeline)
        {
            var context = new MarketRow
            {
                Timestamp = timestamp,
                PriceData = new Dictionary<string, OhlcvBar>(StringComparer.OrdinalIgnoreCase)
            };

            foreach (var symbol in index.Keys)
            {
                index[symbol].TryGetValue(timestamp, out var currentBar);
                
                if (currentBar != null) carry.Update(symbol, currentBar);

                if (carry.TryGetLastBar(symbol, out var lastBar))
                {
                    context.PriceData[symbol] = new OhlcvBar
                    {
                        Open   = lastBar.Open,
                        High   = lastBar.High,
                        Low    = lastBar.Low,
                        Close  = lastBar.Close,
                        Volume = currentBar != null ? currentBar.Volume : 0m 
                    };
                }
            }
            MarketRows.Add(context);
        }
        stopwatch.Stop();
        _logger?.LogDebug("Initialized {Count} market contexts in {ElapsedMilliseconds} ms", MarketRows.Count, stopwatch.ElapsedMilliseconds);
        return MarketRows;
    }
}