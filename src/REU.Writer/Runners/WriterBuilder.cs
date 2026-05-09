using Contracts.Definitions;
using Microsoft.Extensions.Logging;
using Writer.Output;

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
        var outputManager = new Manager(_outputDefinition.OutputPath, runId, _loggerFactory.CreateLogger<Manager>());
        
        _logger.LogDebug("Writer is built with: OutputPath: {OutputPath}, IncludeOhlcvFrames: {IncludeOhlcvFrames}, IncludeTradeLog: {IncludeTradeLog}, IncludeEquityCurve: {IncludeEquityCurve}", _outputDefinition.OutputPath, _outputDefinition.IncludeOhlcvFrames, _outputDefinition.IncludeTradeLog, _outputDefinition.IncludeEquityCurve);
        return new Writer(outputManager, _loggerFactory.CreateLogger<Writer>(), runId, _outputDefinition.OutputPath, _loggerFactory, _outputDefinition.IncludeOhlcvFrames, _outputDefinition.IncludeTradeLog, _outputDefinition.IncludeEquityCurve);
    }
}