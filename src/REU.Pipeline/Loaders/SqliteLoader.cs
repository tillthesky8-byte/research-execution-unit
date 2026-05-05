using Contracts.Definitions;
using Contracts.Enums;
using Contracts.Interfaces;
using Contracts.Rows;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;
using Dapper;

namespace Pipeline.Loaders;

public class SqliteLoader : ILoader
{
    private readonly string _connectionString;
    private readonly ILogger<SqliteLoader> _logger;
    public SqliteLoader(string connectionString, ILogger<SqliteLoader> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task<IReadOnlyList<FactorRow>> LoadFactorDataAsync(FactorDefinition factor, Timeframe timeframe, DateTime start, DateTime end)
    {
        using var connection = new SqliteConnection(_connectionString);
        var query = @"SELECT timestamp, value FROM external
                        WHERE ticker = @FactorName AND timeframe = @Timeframe
                        AND timestamp >= @Start AND timestamp <= @End
                        ORDER BY timestamp ASC";
        var parameters = new
        {
            FactorName = factor.Name,
            Timeframe = timeframe.ToString(),
            Start = start,
            End = end
        };
        var factorRows = await connection.QueryAsync<FactorRow>(query, parameters);
        _logger.LogDebug("Loaded {Count} rows for factor {FactorName} with timeframe {Timeframe} between {Start} and {End}", factorRows.AsList().Count, factor.Name, timeframe, start, end);
        
        return factorRows.AsList();
    }

    public async Task<IReadOnlyList<OhlcvRow>> LoadOhlcvDataAsync(InstrumentDefinition instrument, Timeframe timeframe, DateTime start, DateTime end)
    {
        using var connection = new SqliteConnection(_connectionString);
        var query = @"SELECT timestamp, open, high, low, close, volume FROM ohlcv
                        WHERE ticker = @InstrumentSymbol AND timeframe = @Timeframe
                        AND timestamp >= @Start AND timestamp <= @End
                        ORDER BY timestamp ASC";
        var parameters = new
        {
            InstrumentSymbol = instrument.Symbol,
            Timeframe = timeframe.ToString(),
            Start = start,
            End = end
        };
        var ohlcvRows = await connection.QueryAsync<OhlcvRow>(query, parameters);
        _logger.LogDebug("Loaded {Count} rows for instrument {InstrumentSymbol} with timeframe {Timeframe} between {Start} and {End}", ohlcvRows.AsList().Count, instrument.Symbol, timeframe, start, end);
        
        return ohlcvRows.AsList();
    }

}