using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;
using Contracts;
using Microsoft.Extensions.Logging;

namespace Simulator.Runners;

public class Simulator : ISimulator
{
    private readonly ILogger<Simulator>? _logger;
    private readonly IStrategy _strategy;
    private readonly IBroker _broker;
    private readonly Portfolio _portfolio;
    private readonly IRecorder _recorder;

    public Simulator(ILogger<Simulator>? logger, IStrategy strategy, IBroker broker, Portfolio portfolio, IRecorder recorder)
    {
        _logger    = logger;
        _strategy  = strategy;
        _broker    = broker;
        _portfolio = portfolio;
        _recorder  = recorder;
    }

    public SimulationResult Run(IEnumerable<MarketRow> marketData)
    {
        //=========================================================================
        // SIMULATION LOOP
        //=========================================================================

        for (int i = 0; i < marketData.Count() - 1; i++)
        {
            var context = marketData.ElementAt(i);

            if (i > 0 && marketData.ElementAt(i - 1).Timestamp > context.Timestamp) throw new InvalidOperationException($"Market data is not sorted by timestamp at index {i}. Timestamp {context.Timestamp} is earlier than previous timestamp {marketData.ElementAt(i - 1).Timestamp}.");
            
            _logger?.LogTrace(LogMessages.OnNewTickPortfolioOverview(_portfolio.GetEquity(context), _portfolio.Cash));

            _broker.ProcessOrders(context, _portfolio);

            var orders = _strategy.OnTick(context, _portfolio);

            foreach (var order in orders)            
                _broker.SubmitOrder(order, context, _portfolio);
            

            _recorder.Record(context.Timestamp, _portfolio.GetEquity(context));
        }

        //=========================================================================
        // FINALIZE
        //=========================================================================
        _recorder.AppendTrades(_portfolio.TradeHistory);
        return _recorder.BuildResult();
    }
    
}