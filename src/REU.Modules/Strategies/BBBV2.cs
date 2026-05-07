using Contracts.Enums;
using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;
using Microsoft.Extensions.Logging; 
using Modules.Indicators;

namespace Modules.Strategies;

public class BBBV2(ILogger<BBBV2> logger) : IStrategy
{
    // BBBV2 = Bollinger Band Breakout Version 2
    // Buy when price breaks above upper band, sell when it breaks below lower band, but with different periods for middle band and side bands.

    private bool _initialized = false;
    private readonly ILogger<BBBV2> _logger = logger;
    private int _middlePeriod;
    private int _sidePeriod;
    private decimal _stdDevMultiplier;
    private string? _source;
    private BollingerBands? _bb;

    public void Initialize(IReadOnlyDictionary<string, string> parameters)
    {
        _middlePeriod = parameters.TryGetValue("middlePeriod", out var middlePeriodStr) 
            ? int.Parse(middlePeriodStr) : 20;

        _sidePeriod = parameters.TryGetValue("sidePeriod", out var sidePeriodStr) 
            ? int.Parse(sidePeriodStr) : 20;

        _stdDevMultiplier = parameters.TryGetValue("stdm", out var stdDevStr)
            ? decimal.Parse(stdDevStr) : 2m;
        
        _source = parameters.TryGetValue("source", out var sourceStr)
            ? sourceStr : "close";

        _bb = new BollingerBands(_middlePeriod, _sidePeriod, _stdDevMultiplier, _source);

        _initialized = true;
    }

    public IEnumerable<OrderRequest> OnTick(MarketContext context, IReadOnlyPortfolio portfolio)
    {
        if (!_initialized)
            throw new InvalidOperationException("Strategy not initialized. Call Initialize() before using the strategy.");
        
        foreach(var (symbol, bar) in context.PriceData)
        {
            _bb?.Update(bar, symbol);
            if (!_bb?.IsReady(symbol) ?? true) continue;

            var hasPosition = portfolio.Positions.TryGetValue(symbol, out var position);
            var allocation = portfolio.Cash * 1m; 
            var quantity = Math.Floor(allocation / bar.Close);

            if (!hasPosition || position!.Quantity == 0) 
            {
                if (bar.Close > _bb!.UpperBand(symbol))
                    yield return new OrderRequest(symbol, OrderSide.Buy, OrderType.Market, quantity);

                else if (bar.Close < _bb!.LowerBand(symbol))
                    yield return new OrderRequest(symbol, OrderSide.Sell, OrderType.Market, quantity); 

            }
            else
            {
                if (position.Quantity > 0 && bar.Close < _bb!.MiddleBand(symbol))
                    yield return new OrderRequest(symbol, OrderSide.Sell, OrderType.Market, position.Quantity); 
                
                else if (position.Quantity < 0 && bar.Close > _bb!.MiddleBand(symbol))
                    yield return new OrderRequest(symbol, OrderSide.Buy, OrderType.Market, -position.Quantity); 
            }
        }
    }
}