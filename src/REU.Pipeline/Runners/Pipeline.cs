using Contracts.Definitions;
using Contracts.Enums;
using Contracts.Interfaces;
using Contracts.Rows;

namespace Pipeline.Runners;

public class Pipeline : IPipeline
{
    private readonly ILoader _loader;
    private readonly IFuser _fuser;
    private readonly InstrumentDefinition[] _instruments;
    private readonly FactorDefinition[] _factors;
    private readonly Timeframe _ohlcvTimeframe;
    private readonly Timeframe _factorTimeframe;
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public Pipeline(ILoader loader, IFuser fuser, DatasetDefinition datasetDefinition)
    {
        _loader = loader;
        _fuser = fuser;
        _instruments = datasetDefinition.Instruments;
        _factors = datasetDefinition.Factors;
        _ohlcvTimeframe = datasetDefinition.Timeframe;
        _factorTimeframe = datasetDefinition.Timeframe;
        _startDate = datasetDefinition.StartDate;
        _endDate = datasetDefinition.EndDate;
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
            var factorData = await _loader.LoadFactorDataAsync(factor, _factorTimeframe, _startDate, _endDate);
            factorDataBySymbol[factor.Name] = factorData;
        }

        var marketContext = _fuser.Fuse(ohlcvDataBySymbol, factorDataBySymbol);
        return marketContext;
    }
}