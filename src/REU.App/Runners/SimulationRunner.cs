using Contracts.Definitions;
using Microsoft.Extensions.Logging;
using Pipeline.Runners;
using Simulator.Runners;
using Contracts.Enums;
using Contracts.Configs;

namespace App.Runners;

public sealed class SimulationRunner
{   private readonly string              _runId;
    private readonly RunConfig           _runConfig;
    private readonly ILoggerFactory      _loggerFactory;
    private readonly PipelineDefinition  _pipelineDefinition;
    private readonly SimulatorDefinition _simulatorDefinition;

    public SimulationRunner(RunConfig runConfig, ILoggerFactory loggerFactory)
    {
        _runConfig = runConfig;
        _loggerFactory       = loggerFactory;
        _pipelineDefinition  = runConfig.PipelineDefinition  ?? throw new ArgumentException("PipelineDefinition must be provided in RunConfig.");
        _simulatorDefinition = runConfig.SimulatorDefinition ?? throw new ArgumentException("SimulatorDefinition must be provided in RunConfig.");
        _runId = runConfig.RunId;
    }

    public async Task Run()
    {
        var pipeline = new PipelineBuilder
          (
              _loggerFactory.CreateLogger<PipelineBuilder>(),
              _loggerFactory,
              _pipelineDefinition
          )
          .BuildPipeline();

        var simulator = new SimulatorBuilder
        (
            _loggerFactory.CreateLogger<SimulatorBuilder>(),
            _loggerFactory,
            _simulatorDefinition
        )
        .BuildSimulator();


        var marketData = await pipeline.ExecuteAsync();

        var simulationResult = simulator.Run(marketData);

        if (_pipelineDefinition.IncludeTradeLog    && _pipelineDefinition.WriterType != WriterType.None) 
            await pipeline.WriteTradeLogAsync(simulationResult.Trades, _runId);

        if (_pipelineDefinition.IncludeEquityCurve && _pipelineDefinition.WriterType != WriterType.None) 
            await pipeline.WriteEquityCurveAsync(simulationResult.EquityCurve, _runId);

        if (_pipelineDefinition.IncludeMarketFrame && _pipelineDefinition.WriterType != WriterType.None) 
            await pipeline.WriteFrameAsync(marketData, _runId);
        
        var report = simulationResult.ToString();

        Console.WriteLine();
        Console.WriteLine(report);
        Console.WriteLine();
    }

}