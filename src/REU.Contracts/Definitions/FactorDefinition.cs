using Contracts.Enums;
namespace Contracts.Definitions;

public record FactorDefinition
{
    public required string Name { get; init; }
    public required Timeframe Timeframe { get; init; }
}