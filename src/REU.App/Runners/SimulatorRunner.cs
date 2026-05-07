using Contracts.Definitions;
using Microsoft.Extensions.Logging;
using Pipeline.Runners;
using Simulator.Runners;
using Contracts.Enums;

namespace App.Runners;

public sealed class SimulatorRunner
{
    private readonly ILoggerFactory _loggerFactory;


    public SimulatorRunner(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public async Task Run(SimulatorDefinition simulatorDefinition, PipelineDefinition pipelineDefinition)
    {
        var pipeline = new PipelineBuilder
          (
              _loggerFactory.CreateLogger<PipelineBuilder>(),
              _loggerFactory,
              pipelineDefinition
          )
          .BuildPipeline();

        var simulator = new SimulatorBuilder
        (
            _loggerFactory.CreateLogger<SimulatorBuilder>(),
            _loggerFactory,
            simulatorDefinition
        )
        .BuildSimulator();


        var marketData = await pipeline.ExecuteAsync();

        if (pipelineDefinition.WriterType != WriterType.None) await pipeline.WriteFrameAsync(marketData);

        var simulationResult = simulator.Run(marketData);

        if (pipelineDefinition.WriterType != WriterType.None) await pipeline.WriteTradeLogAsync(simulationResult.Trades);
        
        var report = simulationResult.ToString();
        
        Console.WriteLine();
        Console.WriteLine(report);
    }

}