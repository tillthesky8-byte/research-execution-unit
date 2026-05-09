using Contracts.Definitions;
using Microsoft.Extensions.Logging;
using Writer.Output;
using Writer.Factories;
namespace Writer.Runners;

public class WriterBuilder
{
    private readonly OutputDefinition _outputDefinition;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<WriterBuilder> _logger;
    public WriterBuilder(ILogger<WriterBuilder> logger, ILoggerFactory loggerFactory, OutputDefinition outputDefinition)
    {
        _outputDefinition = outputDefinition;
        _loggerFactory = loggerFactory;
        _logger = logger;
    }

    public Writer BuildWriter(string runId)
    {
        var outputManager = ManagerFactory.CreateManager(_outputDefinition.ManagerType, _outputDefinition.OutputPath, runId, _loggerFactory);
        var indexer = IndexerFactory.CreateIndexer(_outputDefinition.IndexerType, _outputDefinition.OutputPath, _loggerFactory);
        
        _logger.LogDebug("Writer is built with: OutputPath: {OutputPath}, IncludeOhlcvFrames: {IncludeOhlcvFrames}, IncludeTradeLog: {IncludeTradeLog}, IncludeEquityCurve: {IncludeEquityCurve}", _outputDefinition.OutputPath, _outputDefinition.IncludeOhlcvFrames, _outputDefinition.IncludeTradeLog, _outputDefinition.IncludeEquityCurve);
        return new Writer
        (
            outputManager:      outputManager,
            indexer:            indexer,
            logger:             _loggerFactory.CreateLogger<Writer>(),
            runId:              runId,
            outputDirectory:    _outputDefinition.OutputPath,
            includeOhlcvFrames: _outputDefinition.IncludeOhlcvFrames,
            includeTradeLog:    _outputDefinition.IncludeTradeLog,
            includePositionLog: _outputDefinition.IncludeEquityCurve
        );
    }
}
