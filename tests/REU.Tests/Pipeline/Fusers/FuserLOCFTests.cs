using Contracts.Rows;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Pipeline.Fusers;

namespace REU.Tests.Pipeline.Fusers;

public class FuserLOCFTests
{
    [Test]
    public void Fuse_WithSingleOhlcvSeriesAndNoFactors_InitializesMarketContextsFromPriceData()
    {
        var fuser = new FuserLOCF(new TestLogger<FuserLOCF>());
        var ohlcvData = new Dictionary<string, IReadOnlyList<OhlcvRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["US100"] = new[]
            {
                new OhlcvRow
                {
                    Timestamp = new DateTime(2026, 1, 1, 0, 0, 0),
                    Open = 100m,
                    High = 110m,
                    Low = 95m,
                    Close = 105m,
                    Volume = 1_000m
                },
                new OhlcvRow
                {
                    Timestamp = new DateTime(2026, 1, 2, 0, 0, 0),
                    Open = 105m,
                    High = 112m,
                    Low = 101m,
                    Close = 109m,
                    Volume = 1_250m
                }
            }
        };

        var marketContexts = fuser.Fuse(
            ohlcvData,
            new Dictionary<string, IReadOnlyList<FactorRow>>(StringComparer.OrdinalIgnoreCase));

        marketContexts.Should().HaveCount(2);

        marketContexts[0].Timestamp.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0));
        marketContexts[0].PriceData.Should().ContainKey("US100");
        marketContexts[0].PriceData["US100"].Should().BeEquivalentTo(new OhlcvBar
        {
            Open = 100m,
            High = 110m,
            Low = 95m,
            Close = 105m,
            Volume = 1_000m
        });
        marketContexts[0].FactorData.Should().BeEmpty();

        marketContexts[1].Timestamp.Should().Be(new DateTime(2026, 1, 2, 0, 0, 0));
        marketContexts[1].PriceData.Should().ContainKey("US100");
        marketContexts[1].PriceData["US100"].Should().BeEquivalentTo(new OhlcvBar
        {
            Open = 105m,
            High = 112m,
            Low = 101m,
            Close = 109m,
            Volume = 1_250m
        });
        marketContexts[1].FactorData.Should().BeEmpty();
    }

    [Test]
    public void Fuse_WithOhlcvAndFactorData_AppliesLastObservationCarryForward()
    {
        var fuser = new FuserLOCF(new TestLogger<FuserLOCF>());
        var ohlcvData = new Dictionary<string, IReadOnlyList<OhlcvRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["US100"] = new[]
            {
                new OhlcvRow
                {
                    Timestamp = new DateTime(2026, 1, 1, 0, 0, 0),
                    Open = 100m,
                    High = 110m,
                    Low = 95m,
                    Close = 105m,
                    Volume = 1_000m
                },
                new OhlcvRow
                {
                    Timestamp = new DateTime(2026, 1, 2, 0, 0, 0),
                    Open = 105m,
                    High = 112m,
                    Low = 101m,
                    Close = 109m,
                    Volume = 1_250m
                },
                new OhlcvRow
                {
                    Timestamp = new DateTime(2026, 1, 3, 0, 0, 0),
                    Open = 109m,
                    High = 115m,
                    Low = 104m,
                    Close = 111m,
                    Volume = 1_100m
                }
            }
        };

        var factorData = new Dictionary<string, IReadOnlyList<FactorRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["interest_rate"] = new[]
            {
                new FactorRow
                {
                    Timestamp = new DateTime(2026, 1, 1, 12, 0, 0),
                    Value = 2.5m
                },
                new FactorRow
                {
                    Timestamp = new DateTime(2026, 1, 3, 8, 0, 0),
                    Value = 2.75m
                }
            }
        };

        var marketContexts = fuser.Fuse(ohlcvData, factorData);

        marketContexts.Should().HaveCount(3);

        // First context: factor not yet available
        marketContexts[0].Timestamp.Should().Be(new DateTime(2026, 1, 1, 0, 0, 0));
        marketContexts[0].FactorData["interest_rate"].Should().BeNull();

        // Second context: factor at 2.5m (LOCF from first factor at 2026-01-01 12:00)
        marketContexts[1].Timestamp.Should().Be(new DateTime(2026, 1, 2, 0, 0, 0));
        marketContexts[1].FactorData["interest_rate"].Should().Be(2.5m);

        // Third context: factor still at 2.5m (second factor at 2026-01-03 08:00 hasn't arrived yet at midnight)
        marketContexts[2].Timestamp.Should().Be(new DateTime(2026, 1, 3, 0, 0, 0));
        marketContexts[2].FactorData["interest_rate"].Should().Be(2.5m);
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