using Contracts.Enums;

namespace Contracts.Definitions;

public sealed record StrategyDefinition(
    StrategyType Type,
    Dictionary<string, string> Parameters
);