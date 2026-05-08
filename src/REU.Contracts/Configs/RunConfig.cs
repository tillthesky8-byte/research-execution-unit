using Contracts.Definitions;
using Microsoft.Extensions.Logging;
namespace Contracts.Configs;
public record RunConfig
{
    public PipelineDefinition? PipelineDefinition { get; init; }
    public SimulatorDefinition? SimulatorDefinition { get; init; }
    public required ILoggerFactory LoggerFactory { get; init; }
}