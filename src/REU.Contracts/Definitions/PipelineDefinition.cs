using Contracts.Enums;

namespace Contracts.Definitions;

public record PipelineDefinition
{
    public required DatasetDefinition Dataset { get; init; }
    public LoaderType LoaderType { get; init; } = LoaderType.Sqlite;
    public FuserType FuserType { get; init; } = FuserType.LastObservationCarriedForward;
}