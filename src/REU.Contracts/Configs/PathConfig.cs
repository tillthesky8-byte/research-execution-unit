namespace Contracts.Configs;

public record PathConfig
{
    public required string Source { get; init; }
    public required string OutputPath { get; init; }
}