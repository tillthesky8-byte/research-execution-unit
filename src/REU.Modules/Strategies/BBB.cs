using Contracts.Enums;
using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;
using Modules.Indicators;

namespace Modules.Strategies;

public class BBB : IStrategy
{
    // BBB = Bollinger Band Breakout
    // Buy when price breaks above upper band, sell when it breaks below lower band. Close at a cross of the middle band. 

    private readonly BollingerBands _bb;
    public BBB(decimal stdDevMultiplier, int period, string source)
    {
        _bb = new BollingerBands(period, stdDevMultiplier, source);
    }

    public IEnumerable<OrderRequest> OnTick(MarketContext context, IReadOnlyPortfolio portfolio)
    {
        foreach(var (symbol, bar) in context.PriceData)
        {
            _bb.Update(bar, symbol);
            if (!_bb.IsReady(symbol)) continue;

            var hasPosition = portfolio.Positions.TryGetValue(symbol, out var position);
            var allocation = portfolio.Cash * 0.1m; 
            var quantity = Math.Floor(allocation / bar.Close);

            if (!hasPosition || position!.Quantity == 0) 
            {
                if (bar.Close > _bb.UpperBand(symbol))
                    yield return new OrderRequest(symbol, OrderSide.Buy, OrderType.Market, quantity);

                else if (bar.Close < _bb.LowerBand(symbol))
                    yield return new OrderRequest(symbol, OrderSide.Sell, OrderType.Market, quantity); 

            }
            else
            {
                if (position.Quantity > 0 && bar.Close < _bb.MiddleBand(symbol))
                    yield return new OrderRequest(symbol, OrderSide.Sell, OrderType.Market, position.Quantity); 
                
                else if (position.Quantity < 0 && bar.Close > _bb.MiddleBand(symbol))
                    yield return new OrderRequest(symbol, OrderSide.Buy, OrderType.Market, position.Quantity);
            }

        }
    }
}