using Contracts.Enums;
using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;
using Microsoft.Extensions.Logging; 
using Modules.Indicators;

namespace Modules.Strategies;

public class BBBV2(ILogger<BBBV2> logger) : IStrategy
{
    private          bool            _initialized = false ;
    private readonly ILogger<BBBV2>  _logger      = logger;
    private          int             _middlePeriod        ;
    private          int             _sidePeriod          ;
    private          decimal         _stdDevMultiplier    ;
    private          string?         _source              ;
    private          BollingerBands? _bb                  ;

    public void Initialize(IReadOnlyDictionary<string, string> parametersRaw)
    {
        _logger.LogDebug("Parameters received for BBBV2 initialization: {Parameters}", string.Join(", ", parametersRaw.Select(p => $"{p.Key}={p.Value}")));
        var parameters   = parametersRaw.ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);
        
        _middlePeriod     = parameters.TryGetValue("middlePeriod", out var middlePeriodStr) 
            ? int.Parse(middlePeriodStr) : throw new ArgumentException("middlePeriod parameter is required for BBBV2 strategy.");

        _sidePeriod       = parameters.TryGetValue("sidePeriod", out var sidePeriodStr) 
            ? int.Parse(sidePeriodStr)   : throw new ArgumentException("sidePeriod parameter is required for BBBV2 strategy.");

        _stdDevMultiplier = parameters.TryGetValue("stdm", out var stdDevStr)
            ? decimal.Parse(stdDevStr)   : throw new ArgumentException("stdm parameter is required for BBBV2 strategy.");
        
        _source = parameters.TryGetValue("source", out var sourceStr)
            ? sourceStr : "close";

        _bb = new BollingerBands(_middlePeriod, _sidePeriod, _stdDevMultiplier, _source);

        

        _initialized = true;
        _logger.LogDebug("{name} strategy initialized with parameters: {Parameters}", nameof(BBB), string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}")));
    }

    public IEnumerable<OrderRequest> OnTick(MarketRow context, IReadOnlyPortfolio portfolio)
    {
        if (!_initialized)
            throw new InvalidOperationException("Strategy not initialized. Call Initialize() before using the strategy.");
        
        foreach(var (symbol, bar) in context.PriceData)
        {
            _bb?.Update(bar, symbol);
            if (!_bb?.IsReady(symbol) ?? true) continue;

            var hasPosition = portfolio.Positions.TryGetValue(symbol, out var position);
            var allocation  = portfolio.Cash     * 1m                                  ;
            var quantity    = Math     .Floor     (allocation / bar.Close)             ;
            if (quantity == 0) continue;
            if (!hasPosition || position!.Quantity == 0) 
            {
                if      (bar.Close > _bb!.UpperBand(symbol))
                    yield return new OrderRequest(symbol, OrderSide.Buy,  OrderType.Market, quantity);

                else if (bar.Close < _bb!.LowerBand(symbol))
                    yield return new OrderRequest(symbol, OrderSide.Sell, OrderType.Market, quantity); 

            }
            else
            {
                if      (position.Quantity > 0 && bar.Close < _bb!.MiddleBand(symbol))
                    yield return new OrderRequest(symbol, OrderSide.Sell, OrderType.Market, position.Quantity); 
                
                else if (position.Quantity < 0 && bar.Close > _bb!.MiddleBand(symbol))
                    yield return new OrderRequest(symbol, OrderSide.Buy,  OrderType.Market, -position.Quantity); 
            }
        }
    }
}