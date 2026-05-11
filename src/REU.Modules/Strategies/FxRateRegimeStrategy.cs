
using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;
using Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace Modules.Strategies;

public class FxRateRegimeStrategy(ILogger<FxRateRegimeStrategy>? logger) : IStrategy
{
    private          bool                           _initialized            = false ;
    private readonly ILogger<FxRateRegimeStrategy>? _logger                 = logger;
    private          decimal?                       _previousInterestRate   = null  ;
    private          decimal?                       _currentInterestRate    = null  ;   
    private          decimal                        _upperThreshold         = 2m;
    private          decimal                        _lowerThreshold         = 2m;
    public void Initialize(IReadOnlyDictionary<string, string> parameters)
    {
        var parametersDict = parameters.ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);
        _logger?.LogDebug("Parameters received for FxRateRegimeStrategy initialization: {Parameters}", string.Join(", ", parametersDict.Select(p => $"{p.Key}={p.Value}")));
        
        _upperThreshold = parameters != null && parameters.TryGetValue("upperThreshold", out var upperStr) 
            ? decimal.Parse(upperStr) : _upperThreshold;
        
        _lowerThreshold = parameters != null && parameters.TryGetValue("lowerThreshold", out var lowerStr)
            ? decimal.Parse(lowerStr) : _lowerThreshold;

        _initialized = true;
        _logger?.LogDebug("FxRateRegimeStrategy initialized with parameters: {Parameters}", parameters != null ? string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}")) : "None");
    }

    public IEnumerable<OrderRequest> OnTick(MarketRow context, IReadOnlyPortfolio portfolio)
    {
        if (!_initialized)
            throw new InvalidOperationException("Strategy not initialized. Call Initialize() before using the strategy.");

        foreach(var (symbol, bar) in context.PriceData)
        {
            var hasPosition = portfolio.Positions.TryGetValue(symbol, out var position);
            var allocation  = portfolio.Cash     * 1m                                  ;
            var quantity    = Math     .Floor     (allocation / bar.Close)             ;
            if (quantity == 0) continue;

            var interestRateChange = context.FactorData.TryGetValue("us_interest_rate", out _currentInterestRate)  && _previousInterestRate.HasValue
                    ? _currentInterestRate - _previousInterestRate.Value : (decimal?)null;
            
            if (!hasPosition || position!.Quantity == 0)
            {
                if      (_currentInterestRate.HasValue && _currentInterestRate.Value > _upperThreshold)
                {
                    yield return new OrderRequest(symbol, OrderSide.Buy, OrderType.Market, quantity);
                }
                else if (_currentInterestRate.HasValue && _currentInterestRate.Value < _lowerThreshold)
                {
                    yield return new OrderRequest(symbol, OrderSide.Sell, OrderType.Market, quantity);
                }
            }
            else
            {
                // EXIT the position when conditions no longer apply, don't scale in
                if (_currentInterestRate.HasValue && _currentInterestRate.Value <= _lowerThreshold && position.Quantity > 0)
                {
                    yield return new OrderRequest(symbol, OrderSide.Sell, OrderType.Market, position.Quantity);
                }
                else if (_currentInterestRate.HasValue && _currentInterestRate.Value >= _upperThreshold && position.Quantity < 0)
                {
                    yield return new OrderRequest(symbol, OrderSide.Buy, OrderType.Market, Math.Abs(position.Quantity));
                }
            }

        }   

        if (_currentInterestRate.HasValue)        {
            _logger?.LogTrace("Current interest rate: {InterestRate}, Previous interest rate: {PreviousInterestRate}", _currentInterestRate.Value, _previousInterestRate);
            _previousInterestRate = _currentInterestRate.Value; 
        } 
    }
}