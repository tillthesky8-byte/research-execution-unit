using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;
using Microsoft.Extensions.Logging;

namespace Pipeline.Writers;

public class JsonWriter : IWriter
{
    private readonly string _outputPath;
    private readonly ILogger<JsonWriter> _logger;

    public JsonWriter(string outputPath, ILogger<JsonWriter> logger)
    {
        _outputPath = outputPath;
        _logger = logger;
    }

    public Task WriteEquityCurveAsync(IEnumerable<EquityPoint> equityCurve)
    {
        var filePath = _outputPath;
        if (string.IsNullOrWhiteSpace(Path.GetExtension(filePath)))
        {
            Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, "equity_curve.json");
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");
        }

        var json = System.Text.Json.JsonSerializer.Serialize(equityCurve);
        File.WriteAllText(filePath, json);
        _logger.LogInformation("Wrote equity curve to {FilePath}", filePath);
        return Task.CompletedTask;
    }

    public Task WriteFrameAsync(IEnumerable<MarketContext> data)
    {
        var filePath = _outputPath;
        if (string.IsNullOrWhiteSpace(Path.GetExtension(filePath)))
        {
            Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, "market_dataframe.json");
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");
        }
        
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        File.WriteAllText(filePath, json);
        _logger.LogInformation("Wrote market frame to {FilePath}", filePath);
        return Task.CompletedTask;
    }

    public Task WriteTradeLogAsync(IEnumerable<Trade> tradeLog)
    {
        var filePath = _outputPath;
        if (string.IsNullOrWhiteSpace(Path.GetExtension(filePath)))
        {
            Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, "trade_log.json");
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");
        }
        
        var json = System.Text.Json.JsonSerializer.Serialize(tradeLog);
        File.WriteAllText(filePath, json);
        _logger.LogInformation("Wrote trade log to {FilePath}", filePath);
        return Task.CompletedTask;
    }
}