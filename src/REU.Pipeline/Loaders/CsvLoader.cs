using Microsoft.Extensions.Logging;
using CsvHelper;
using Contracts.Interfaces;
using Contracts.Rows;
using Contracts.Definitions;
using Contracts.Enums;
using CsvHelper.Configuration;

namespace Pipeline.Loaders;

public class CsvLoader : ILoader
{
    private readonly ILogger<CsvLoader> _logger;
    private readonly string _dataDirectory;
    private const string OhlcvFilePattern = "{0}_{1}_{2}_{3}.csv"; // e.g. AAPL_1m_20260101_20260131.csv
    private const string FactorFilePattern = "{0}_{1}_{2}_{3}.csv"; // e.g. interest_rate_1d_20260101_20261231.csv
    public CsvLoader(ILogger<CsvLoader> logger, string dataDirectory)
    {
        _logger = logger;
        _dataDirectory = dataDirectory;
    }

    public Task<IReadOnlyList<FactorRow>> LoadFactorDataAsync(FactorDefinition factor, Timeframe timeframe, DateTime start, DateTime end)
    {
        var fileName = string.Format(FactorFilePattern, factor.Name, timeframe.ToString(), start.ToString("yyyyMMdd"), end.ToString("yyyyMMdd"));
        var filePath = Path.Combine(_dataDirectory, fileName);
        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Factor data file not found: {FilePath}", filePath);
            return Task.FromResult<IReadOnlyList<FactorRow>>(new List<FactorRow>());
        }

        using var reader = new StreamReader(filePath);

        var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.Replace(" ", "").ToLower(),
        };

        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<FactorRow>().ToList();
        _logger.LogDebug("Loaded {Count} rows for factor {FactorName} with timeframe {Timeframe} between {Start} and {End} from CSV", records.Count, factor.Name, timeframe, start, end);
        
        return Task.FromResult<IReadOnlyList<FactorRow>>(records);
    }

    public Task<IReadOnlyList<OhlcvRow>> LoadOhlcvDataAsync(InstrumentDefinition instrument, Timeframe timeframe, DateTime start, DateTime end)
    {
        var fileName = string.Format(OhlcvFilePattern, instrument.Symbol, timeframe.ToString(), start.ToString("yyyyMMdd"), end.ToString("yyyyMMdd"));
        var filePath = Path.Combine(_dataDirectory, fileName);
        if (!File.Exists(filePath))
        {
            _logger.LogWarning("OHLCV data file not found: {FilePath}", filePath);
            return Task.FromResult<IReadOnlyList<OhlcvRow>>(new List<OhlcvRow>());
        }

        using var reader = new StreamReader(filePath);

        var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.Replace(" ", "").ToLower(),
        };

        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<OhlcvRow>().ToList();
        _logger.LogDebug("Loaded {Count} rows for instrument {InstrumentSymbol} with timeframe {Timeframe} between {Start} and {End} from CSV", records.Count, instrument.Symbol, timeframe, start, end);

        return Task.FromResult<IReadOnlyList<OhlcvRow>>(records);
    }

    public Task<IReadOnlyList<MarketContext>> LoadMarketContextAsync(InstrumentDefinition instrument, FactorDefinition[] factors, Timeframe timeframe, DateTime start, DateTime end)
    {
        throw new NotImplementedException("LoadMarketContextAsync is not implemented in CsvLoader. Please load OHLCV and factor data separately and fuse them using the Fuser.");
    }
}