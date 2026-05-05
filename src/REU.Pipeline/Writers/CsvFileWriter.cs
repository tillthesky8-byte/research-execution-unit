using Contracts.Interfaces;
using Contracts.Rows;
using Microsoft.Extensions.Logging;

namespace Pipeline.Writers;

public class CsvFileWriter : IWriter
{
    private readonly string _filePath;
    private readonly ILogger<CsvFileWriter> _logger;

    public CsvFileWriter(string filePath, ILogger<CsvFileWriter> logger)
    {
        _filePath = filePath;
        _logger = logger;
    }

    public async Task WriteFrameAsync(IEnumerable<MarketContext> data)
    {
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
        
        using var writer = new StreamWriter(_filePath);
        
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
}