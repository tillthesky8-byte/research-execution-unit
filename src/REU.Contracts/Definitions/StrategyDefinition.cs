using Contracts.Enums;

namespace Contracts.Definitions;

public sealed record StrategyDefinition(
    StrategyType Type,
    IReadOnlyDictionary<string, string> Parameters
);