using Contracts.Definitions;
using Contracts.Enums;
using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;
using Microsoft.Extensions.Logging;

namespace Pipeline.Runners;

public class Pipeline : IPipeline
{
    private readonly ILogger<Pipeline> _logger;
    private readonly ILoader _loader;
    private readonly IFuser _fuser;
    private readonly IWriter _writer;
    private readonly InstrumentDefinition[] _instruments;
    private readonly FactorDefinition[] _factors;
    private readonly Timeframe _ohlcvTimeframe;
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public Pipeline(ILoader loader, IFuser fuser, IWriter writer, DatasetDefinition datasetDefinition, ILogger<Pipeline> logger)
    {
        _loader = loader;
        _fuser = fuser;
        _writer = writer;
        _instruments = datasetDefinition.Instruments;
        _factors = datasetDefinition.Factors;
        _ohlcvTimeframe = datasetDefinition.Timeframe;
        _startDate = datasetDefinition.StartDate;
        _endDate = datasetDefinition.EndDate;
        _logger = logger;
    _logger.LogDebug("Factors received: {Factors}", string.Join(", ", _factors.Select(f => f.Name)));
    }
    public async Task<IReadOnlyList<MarketContext>> ExecuteAsync()
    {
        var ohlcvDataBySymbol = new Dictionary<string, IReadOnlyList<OhlcvRow>>(StringComparer.OrdinalIgnoreCase);
        var factorDataBySymbol = new Dictionary<string, IReadOnlyList<FactorRow>>(StringComparer.OrdinalIgnoreCase);

        foreach (var instrument in _instruments)
        {
            var ohlcvData = await _loader.LoadOhlcvDataAsync(instrument, _ohlcvTimeframe, _startDate, _endDate);
            ohlcvDataBySymbol[instrument.Symbol] = ohlcvData;
        }

        foreach (var factor in _factors)
        {
            var factorData = await _loader.LoadFactorDataAsync(factor, factor.Timeframe, _startDate, _endDate);
            factorDataBySymbol[factor.Name] = factorData;
        }

        var marketContext = _fuser.Fuse(ohlcvDataBySymbol, factorDataBySymbol);
        return marketContext;
    }

    public Task WriteEquityCurveAsync(IReadOnlyList<EquityPoint> equityCurve, string runId)
    {
        return _writer.WriteEquityCurveAsync(equityCurve, runId);
    }

    public async Task WriteFrameAsync(IReadOnlyList<MarketContext> marketContexts, string runId)
    {
        if (marketContexts == null || marketContexts.Count == 0) return;
        await _writer.WriteFrameAsync(marketContexts, runId);
    }

    public Task WriteTradeLogAsync(IReadOnlyList<Trade> tradeLog, string runId)
    {
        return _writer.WriteTradeLogAsync(tradeLog, runId);
    }
}