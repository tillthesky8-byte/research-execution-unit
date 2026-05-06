using Contracts.Models;
using Contracts.Interfaces;
using Contracts.Definitions;
using Microsoft.Extensions.Logging;
using Simulator.Factories;
using Simulator.Recorders;

namespace Simulator.Runners;

public class SimulatorBuilder
{
    private readonly ILogger<SimulatorBuilder> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly SimulatorDefinition _simulatorDefinition;

    public SimulatorBuilder(ILogger<SimulatorBuilder> logger, ILoggerFactory loggerFactory, SimulatorDefinition simulatorDefinition)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
        _simulatorDefinition = simulatorDefinition;
    }

    public ISimulator BuildSimulator()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Building simulator...");

        var strategy = StrategyFactory.CreateStrategy(_simulatorDefinition.Strategy.Type, _loggerFactory);
        strategy.Initialize(_simulatorDefinition.Strategy.Parameters);

        var broker = BrokerFactory.CreateBroker(_simulatorDefinition.SlippageModel, _simulatorDefinition.ComissionModel, _loggerFactory);

        var portfolio = PortfolioFactory.CreatePortfolio(_simulatorDefinition.InitialCash);

        var recorder = new Recorder();

        stopwatch.Stop();
        _logger.LogInformation("Simulator built in {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        return new Simulator(_loggerFactory.CreateLogger<Simulator>(), strategy, broker, portfolio, recorder);
    }
}