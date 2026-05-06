using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;
using Microsoft.Extensions.Logging;

namespace Simulator.Runners;

public class Simulator : ISimulator
{
    private readonly ILogger<Simulator> _logger;
    private readonly IStrategy _strategy;
    private readonly IBroker _broker;
    private readonly Portfolio _portfolio;
    private readonly IRecorder _recorder;

    public Simulator(ILogger<Simulator> logger, IStrategy strategy, IBroker broker, Portfolio portfolio, IRecorder recorder)
    {
        _logger = logger;
        _strategy = strategy;
        _broker = broker;
        _portfolio = portfolio;
        _recorder = recorder;
    }

    public SimulationResult Run(IEnumerable<MarketContext> marketData)
    {
        foreach (var context in marketData)
        {
            _broker.ProcessOrders(context, _portfolio);

            var orders = _strategy.OnTick(context, _portfolio);

            foreach (var order in orders)            {
                _broker.SubmitOrder(order, context, _portfolio);
            }

            _recorder.Record(context.Timestamp, _portfolio.GetEquity(context));
        }
        return _recorder.BuildResult();
    }
    
}