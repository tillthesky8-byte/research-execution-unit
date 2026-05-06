using Contracts.Definitions;
using Contracts.Enums;
using Microsoft.Extensions.Logging;
using Pipeline.Runners;

namespace App.Runners;

public sealed class PipelineRunner
{
    private readonly ILoggerFactory _loggerFactory;

    public PipelineRunner(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }
    public async Task RunAsync(PipelineDefinition pipelineDefinition)
    {
        var pipeline = new PipelineBuilder
          (
              _loggerFactory.CreateLogger<PipelineBuilder>(),
              _loggerFactory,
              pipelineDefinition
          )
          .BuildPipeline();

        var data = await pipeline.ExecuteAsync();

        if (pipelineDefinition.WriterType != WriterType.None)
        {
            await pipeline.WriteFrameAsync(data);
        }
    }
}