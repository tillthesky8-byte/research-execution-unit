using Contracts.Configs;
using Contracts.Definitions;
using Contracts.Enums;
using Microsoft.Extensions.Logging;
using Pipeline.Runners;

namespace App.Runners;

public sealed class PipelineRunner
{
    private readonly string _runId;
    private readonly ILoggerFactory _loggerFactory;
    private readonly PipelineDefinition _pipelineDefinition;

    public PipelineRunner(RunConfig runConfig, ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _pipelineDefinition = runConfig.PipelineDefinition ?? throw new ArgumentException("PipelineDefinition must be provided in RunConfig.");
        _runId = runConfig.RunId;

    }
    public async Task RunAsync()
    {
        var pipeline = new PipelineBuilder
          (
              _loggerFactory.CreateLogger<PipelineBuilder>(),
              _loggerFactory,
              _pipelineDefinition
          )
          .BuildPipeline();

        var data = await pipeline.ExecuteAsync();

        if (_pipelineDefinition.WriterType != WriterType.None)
        {
            await pipeline.WriteFrameAsync(data, _runId);
        }
    }
}