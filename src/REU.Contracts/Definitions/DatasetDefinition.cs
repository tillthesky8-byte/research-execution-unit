using Contracts.Enums;

namespace Contracts.Definitions
{
    public class DatasetDefinition
    {
        public required string ConnectionString { get; init; }
        public required InstrumentDefinition[] Instruments { get; init; }
        public required FactorDefinition[] Factors { get; init; }
        public required DateTime StartDate { get; init; }
        public required DateTime EndDate { get; init; }
        public required Timeframe Timeframe { get; init; }
    }
}