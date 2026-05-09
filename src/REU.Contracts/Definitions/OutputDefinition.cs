using Contracts.Enums;

namespace Contracts.Definitions;

public record OutputDefinition
{
    public required string OutputPath { get; init; }
    public required bool IncludeOhlcvFrames { get; init; }
    public required bool IncludeTradeLog { get; init; }
    public required bool IncludeEquityCurve { get; init; }
    public ManagerType ManagerType { get; init; } = ManagerType.Default;
    public IndexerType IndexerType { get; init; } = IndexerType.Default;
}