using Contracts.Definitions;
using Contracts.Enums;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface ILoader
{
    Task<IReadOnlyList<OhlcvRow>> LoadOhlcvDataAsync(InstrumentDefinition instrument, Timeframe timeframe, DateTime start, DateTime end);
    Task<IReadOnlyList<FactorRow>> LoadFactorDataAsync(InstrumentDefinition instrument, Timeframe timeframe, DateTime start, DateTime end);
}