using Contracts.Enums;
using Contracts.Definitions;

namespace Contracts.Definitions;

public record PipelineDefinition
{
    public required DatasetDefinition Dataset { get; init; }
    public required string Source { get; init; }
    public required string OutputPath { get; init; }
    public required bool IncludeMarketFrame { get; init; }
    public required bool IncludeTradeLog { get; init; }
    public required bool IncludeEquityCurve { get; init; }
    public LoaderType LoaderType { get; init; } = LoaderType.Sqlite;
    public FuserType FuserType { get; init; } = FuserType.LastObservationCarriedForward;
    public WriterType WriterType { get; init; } = WriterType.CsvFile;
}