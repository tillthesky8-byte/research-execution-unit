using Contracts.Configs;
using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;
using Microsoft.Extensions.Logging;
using Writer.Indexers;

namespace Writer.Runners;


public sealed class Writer
{
    private readonly IManager        _outputManager;
    private readonly IIndexer        _indexer;
    private readonly ILogger<Writer> _logger;
    private readonly string          _runId;
    private readonly string          _outputDirectory;
    private readonly bool            _includeOhlcvFrames;
    private readonly bool            _includeTradeLog;
    private readonly bool            _includePositionLog;

    public Writer(IManager outputManager, IIndexer indexer, ILogger<Writer> logger, string runId, string outputDirectory, bool includeOhlcvFrames, bool includeTradeLog, bool includePositionLog)
    {
        _outputManager = outputManager;
        _indexer = indexer;
        _logger = logger;
        _runId = runId;
        _outputDirectory = outputDirectory;
        _includeOhlcvFrames = includeOhlcvFrames;
        _includeTradeLog = includeTradeLog;
        _includePositionLog = includePositionLog;
    }

    public bool Write(OutputBundle outputBundle)
    {
        WriteRunConfig(outputBundle.RunConfig ?? throw new ArgumentException("RunConfig must be provided in OutputBundle for writing."));

        _indexer.RecreateIndex();

        if (_includeOhlcvFrames && outputBundle.MarketData is not null)
                WriteFrames(outputBundle.MarketData);
        else     
                _logger.LogDebug("IncludeOhlcvFrames: {IncludeOhlcvFrames}", _includeOhlcvFrames);
                _logger.LogDebug("Skipping OHLCV frame output as MarketData is null");

        if (_includeTradeLog && outputBundle.SimulationResult?.Trades is not null)
                WriteTradeLog(outputBundle.SimulationResult.Trades);

        if (_includePositionLog && outputBundle.SimulationResult?.EquityCurve is not null)
                WriteEquityCurve(outputBundle.SimulationResult.EquityCurve);

        return true;
    }

    private void WriteFrames(IReadOnlyList<MarketRow> marketData)
    {
        var market = new Market(marketData);
        var ohlcvBySymbol = market.ToOhlcvSeries();
        foreach (var kvp in ohlcvBySymbol)
        {
            _logger.LogDebug("Writing OHLCV series for symbol {Symbol} with entries", kvp.Key);
            _outputManager.SaveSeries(kvp.Value, $"ohlcv_{kvp.Key}");
        }
    }

    private void WriteTradeLog(IReadOnlyList<Trade> trades)
    {
        _outputManager.SaveSeries(trades, "trade_log");
    }

    private void WriteEquityCurve(IReadOnlyList<EquityPoint> equityCurve)
    {
        _outputManager.SaveSeries(equityCurve, "equity_curve");
    }

    private void WriteRunConfig(RunConfig runConfig)
    {
        _outputManager.SaveObject(runConfig, "config");
    }

}