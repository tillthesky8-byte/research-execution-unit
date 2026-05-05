using Contracts.Definitions;
using Contracts.Enums;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Pipeline.Loaders;

namespace REU.Tests.Pipeline.Loaders;

public class SqliteLoaderTests
{
    [Test]
    public async Task LoadFactorDataAsync_ReturnsMatchingRowsInTimestampOrder()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"reu-loader-{Guid.NewGuid():N}.db");
        var connectionString = $"Data Source={databasePath}";
        var factor = new FactorDefinition { Name = "interest_rate", Timeframe = Timeframe.D };

        try
        {
            await using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = @"CREATE TABLE external (
                    timestamp TEXT NOT NULL,
                    value REAL NOT NULL,
                    ticker TEXT NOT NULL,
                    timeframe TEXT NOT NULL
                );";
                await createTableCommand.ExecuteNonQueryAsync();

                var insertFirstRow = connection.CreateCommand();
                insertFirstRow.CommandText = @"INSERT INTO external (timestamp, value, ticker, timeframe)
                    VALUES ($timestamp, $value, $ticker, $timeframe);";
                insertFirstRow.Parameters.AddWithValue("$timestamp", "2026-01-02 00:00:00");
                insertFirstRow.Parameters.AddWithValue("$value", 2.25m);
                insertFirstRow.Parameters.AddWithValue("$ticker", factor.Name);
                insertFirstRow.Parameters.AddWithValue("$timeframe", factor.Timeframe.ToString());
                await insertFirstRow.ExecuteNonQueryAsync();

                var insertSecondRow = connection.CreateCommand();
                insertSecondRow.CommandText = @"INSERT INTO external (timestamp, value, ticker, timeframe)
                    VALUES ($timestamp, $value, $ticker, $timeframe);";
                insertSecondRow.Parameters.AddWithValue("$timestamp", "2026-01-01 00:00:00");
                insertSecondRow.Parameters.AddWithValue("$value", 1.75m);
                insertSecondRow.Parameters.AddWithValue("$ticker", factor.Name);
                insertSecondRow.Parameters.AddWithValue("$timeframe", factor.Timeframe.ToString());
                await insertSecondRow.ExecuteNonQueryAsync();
            }

            var loader = new SqliteLoader(connectionString, new TestLogger<SqliteLoader>());

            var rows = await loader.LoadFactorDataAsync(
                factor,
                factor.Timeframe,
                new DateTime(2026, 1, 1, 0, 0, 0),
                new DateTime(2026, 1, 31, 0, 0, 0));

            rows.Should().HaveCount(2);
            rows[0].Timestamp.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0));
            rows[0].Value.Should().Be(1.75m);
            rows[1].Timestamp.Should().Be(new DateTime(2026, 1, 2, 0, 0, 0));
            rows[1].Value.Should().Be(2.25m);
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    [Test]
    public async Task LoadOhlcvDataAsync_ReturnsMatchingRowsInTimestampOrder()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"reu-loader-{Guid.NewGuid():N}.db");
        var connectionString = $"Data Source={databasePath}";
        var instrument = new InstrumentDefinition { Symbol = "US100" };

        try
        {
            await using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = @"CREATE TABLE ohlcv (
                    timestamp TEXT NOT NULL,
                    open REAL NOT NULL,
                    high REAL NOT NULL,
                    low REAL NOT NULL,
                    close REAL NOT NULL,
                    volume REAL NOT NULL,
                    ticker TEXT NOT NULL,
                    timeframe TEXT NOT NULL
                );";
                await createTableCommand.ExecuteNonQueryAsync();

                var insertFirstRow = connection.CreateCommand();
                insertFirstRow.CommandText = @"INSERT INTO ohlcv (timestamp, open, high, low, close, volume, ticker, timeframe)
                    VALUES ($timestamp, $open, $high, $low, $close, $volume, $ticker, $timeframe);";
                insertFirstRow.Parameters.AddWithValue("$timestamp", "2026-01-02 00:00:00");
                insertFirstRow.Parameters.AddWithValue("$open", 105m);
                insertFirstRow.Parameters.AddWithValue("$high", 112m);
                insertFirstRow.Parameters.AddWithValue("$low", 101m);
                insertFirstRow.Parameters.AddWithValue("$close", 109m);
                insertFirstRow.Parameters.AddWithValue("$volume", 1250m);
                insertFirstRow.Parameters.AddWithValue("$ticker", instrument.Symbol);
                insertFirstRow.Parameters.AddWithValue("$timeframe", Timeframe.D.ToString());
                await insertFirstRow.ExecuteNonQueryAsync();

                var insertSecondRow = connection.CreateCommand();
                insertSecondRow.CommandText = @"INSERT INTO ohlcv (timestamp, open, high, low, close, volume, ticker, timeframe)
                    VALUES ($timestamp, $open, $high, $low, $close, $volume, $ticker, $timeframe);";
                insertSecondRow.Parameters.AddWithValue("$timestamp", "2026-01-01 00:00:00");
                insertSecondRow.Parameters.AddWithValue("$open", 100m);
                insertSecondRow.Parameters.AddWithValue("$high", 110m);
                insertSecondRow.Parameters.AddWithValue("$low", 95m);
                insertSecondRow.Parameters.AddWithValue("$close", 105m);
                insertSecondRow.Parameters.AddWithValue("$volume", 1000m);
                insertSecondRow.Parameters.AddWithValue("$ticker", instrument.Symbol);
                insertSecondRow.Parameters.AddWithValue("$timeframe", Timeframe.D.ToString());
                await insertSecondRow.ExecuteNonQueryAsync();
            }

            var loader = new SqliteLoader(connectionString, new TestLogger<SqliteLoader>());

            var rows = await loader.LoadOhlcvDataAsync(
                instrument,
                Timeframe.D,
                new DateTime(2026, 1, 1, 0, 0, 0),
                new DateTime(2026, 1, 31, 0, 0, 0));

            rows.Should().HaveCount(2);
            rows[0].Timestamp.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0));
            rows[0].Open.Should().Be(100m);
            rows[0].High.Should().Be(110m);
            rows[0].Low.Should().Be(95m);
            rows[0].Close.Should().Be(105m);
            rows[0].Volume.Should().Be(1000m);

            rows[1].Timestamp.Should().Be(new DateTime(2026, 1, 2, 0, 0, 0));
            rows[1].Open.Should().Be(105m);
            rows[1].High.Should().Be(112m);
            rows[1].Low.Should().Be(101m);
            rows[1].Close.Should().Be(109m);
            rows[1].Volume.Should().Be(1250m);
        }
        finally
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }

    private sealed class TestLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose()
            {
            }
        }
    }
}