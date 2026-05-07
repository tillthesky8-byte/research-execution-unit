using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;
using Microsoft.Extensions.Logging;

namespace Pipeline.Writers;

public class CsvFileWriter : IWriter
{
    private readonly string _outputPath;
    private readonly ILogger<CsvFileWriter> _logger;

    public CsvFileWriter(string outputPath, ILogger<CsvFileWriter> logger)
    {
        _outputPath = outputPath;
        _logger = logger;
    }

    public async Task WriteFrameAsync(IEnumerable<MarketContext> data)
    {
        var filePath = _outputPath;
        if (string.IsNullOrWhiteSpace(Path.GetExtension(filePath)))
        {
            Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, "market_context.csv");
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");
        }

        var factorColumns = data
            .SelectMany(mc => mc.FactorData
            .Select(f => f.Key))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var header = new List<string>
        {
            "timestamp",
            "symbol",
            "open",
            "high",
            "low",
            "close",
            "volume"
        };
        header.AddRange(factorColumns);
        
        using var writer = new StreamWriter(filePath);
        
        await writer.WriteLineAsync(string.Join(",", header));
        foreach (var marketContext in data)
        {
            foreach (var priceData in marketContext.PriceData)
            {
                var line = new List<string>
                {
                    marketContext.Timestamp.ToString("o"),
                    priceData.Key,
                    priceData.Value.Open.ToString(),
                    priceData.Value.High.ToString(),
                    priceData.Value.Low.ToString(),
                    priceData.Value.Close.ToString(),
                    priceData.Value.Volume.ToString()
                };
                line.AddRange(factorColumns.Select(fc => marketContext.FactorData.TryGetValue(fc, out var factorValue) ? factorValue?.ToString() ?? "" : ""));
                await writer.WriteLineAsync(string.Join(",", line));
                _logger.LogTrace("Wrote line for symbol {Symbol} at timestamp {Timestamp}", priceData.Key, marketContext.Timestamp);
            }
        }
        return;
    }
    public async Task WriteTradeLogAsync(IEnumerable<Trade> tradeLog)
    {
        var filePath = _outputPath;
        if (string.IsNullOrWhiteSpace(Path.GetExtension(filePath)))
        {
            Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, "trade_log.csv");
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");
        }

        using var writer = new StreamWriter(filePath);
        var header = new List<string>
        {
            "timestamp",
            "symbol",
            "side",
            "action",
            "quantity",
            "price"
        };
        await writer.WriteLineAsync(string.Join(",", header));
        
        foreach (var trade in tradeLog)
        {
            var line = new List<string>
            {
                trade.Timestamp.ToString("o"),
                trade.Symbol,
                trade.Side.ToString(),
                trade.Action.ToString(),
                trade.Quantity.ToString(),
                trade.Price.ToString()
            };
            await writer.WriteLineAsync(string.Join(",", line));
            _logger.LogTrace("Wrote trade log line for symbol {Symbol} at timestamp {Timestamp}", trade.Symbol, trade.Timestamp);
        }
    }
}