namespace Contracts.Definitions;

public record InstrumentDefinition
{
    public required string Symbol { get; init; }
}