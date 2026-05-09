using Contracts.Definitions;

namespace Contracts.Configs;
public record RunConfig
{
    public string RunId { get; } = $"{DateTime.UtcNow:yyyyMMdd_HH-mm-ss-fff}";
    public PipelineDefinition? PipelineDefinition { get; init; }
    public SimulatorDefinition? SimulatorDefinition { get; init; }
    public OutputDefinition? OutputDefinition { get; init; }
}