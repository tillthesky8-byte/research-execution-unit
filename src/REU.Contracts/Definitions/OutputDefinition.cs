using Contracts.Enums;

namespace Contracts.Definitions;

public record OutputDefinition
{
    public required OutputTarget Target { get; init; }
    public OutputFormat Format { get; init; } = OutputFormat.Csv;
    public required string Path { get; init; }
}