using Contracts.Definitions;
using Contracts.Interfaces;
using Pipeline.Factories;
using Microsoft.Extensions.Logging;


namespace Pipeline.Runners;

public class PipelineBuilder
{
    private readonly ILogger<PipelineBuilder> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly PipelineDefinition _pipelineDefinition;

    public PipelineBuilder(ILogger<PipelineBuilder> logger, ILoggerFactory loggerFactory, PipelineDefinition pipelineDefinition)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
        _pipelineDefinition = pipelineDefinition;
    }

    public IPipeline BuildPipeline()
    {

        var loader = LoaderFactory.CreateLoader(_pipelineDefinition.LoaderType, _pipelineDefinition.Dataset.ConnectionString, _loggerFactory);
        var fuser = FuserFactory.CreateFuser(_pipelineDefinition.FuserType, _loggerFactory);
        var writer = WriterFactory.CreateWriter(_pipelineDefinition.WriterType, _pipelineDefinition.Output.Path, _loggerFactory);

        return new Pipeline(loader, fuser, writer, _pipelineDefinition.Dataset);
    }
}

    