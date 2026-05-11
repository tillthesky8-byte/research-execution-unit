using Contracts.Definitions;
using Microsoft.Extensions.Logging;
using Contracts.Models;
using Pipeline.Runners;
using Simulator.Runners;
using Contracts.Configs;
using Writer.Runners;
using Dapper;

namespace App.Runners;

public sealed class SimulationRunner
{   private readonly string              _runId;
    private readonly RunConfig           _runConfig;
    private readonly ILoggerFactory      _loggerFactory;
    private readonly PipelineDefinition  _pipelineDefinition;
    private readonly SimulatorDefinition _simulatorDefinition;
    private readonly OutputDefinition    _outputDefinition;
    private readonly ILogger<SimulationRunner>? _logger;

    public SimulationRunner(RunConfig runConfig, ILoggerFactory loggerFactory)
    {
        _runConfig = runConfig;
        _loggerFactory       = loggerFactory;
        _pipelineDefinition  = runConfig.PipelineDefinition  ?? throw new ArgumentException("PipelineDefinition must be provided in RunConfig.");
        _simulatorDefinition = runConfig.SimulatorDefinition ?? throw new ArgumentException("SimulatorDefinition must be provided in RunConfig.");
        _outputDefinition    = runConfig.OutputDefinition    ?? throw new ArgumentException("OutputDefinition must be provided in RunConfig.");
        _runId               = runConfig.RunId               ?? throw new ArgumentException("RunId must be provided in RunConfig.");
        _logger              = loggerFactory.CreateLogger<SimulationRunner>();
    }

    public async Task Run()
    {
        //=========================================================================
        // BUILD COMPONENTS
        //=========================================================================
        var pipeline  = new PipelineBuilder
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

        var writer    = new WriterBuilder
        (
            _loggerFactory.CreateLogger<WriterBuilder>(),
            _loggerFactory,
            _outputDefinition
        )
        .BuildWriter(_runId);

        
        
        //=========================================================================
        // EXECUTION
        //=========================================================================

        var marketData       = await pipeline.ExecuteAsync();

        var simulationResult = simulator.Run(marketData);

        var writerOutput     = writer.Write(new OutputBundle
        {
            MarketData       = marketData.AsList(),
            SimulationResult = simulationResult,
            RunConfig        = _runConfig
        });

        
        
        //=========================================================================
        // REPORTING
        //=========================================================================

        var report           = simulationResult.ToString();

        Console.WriteLine();
        Console.WriteLine(report);
        Console.WriteLine();
    }

}