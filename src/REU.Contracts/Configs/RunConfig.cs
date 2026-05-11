using Contracts.Definitions;

namespace Contracts.Configs;
public record RunConfig
{
    public string? RunId { get; set; }
    public DateTime RunDate { get; set ;}
    public PipelineDefinition? PipelineDefinition { get; init; }
    public SimulatorDefinition? SimulatorDefinition { get; init; }
    public OutputDefinition? OutputDefinition { get; init; }
    public void SetRunId()   => RunId = $"{DateTime.UtcNow:yyyyMMdd_HH-mm-ss-fff}";
    public void SetRunDate() => RunDate = DateTime.UtcNow;

}