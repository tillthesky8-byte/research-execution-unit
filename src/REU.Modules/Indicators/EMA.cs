using Contracts.Rows;

namespace Modules.Indicators;

public class EMA
{
    private class SymbolState
    {
        public decimal EMAValue { get; set; }
        public int BarCount { get; set; }
        public decimal SumForWarmup { get; set; }
    }

    private readonly int _period;
    private readonly decimal _smoothing;
    private readonly string _source;
    private readonly decimal _multiplier;
    private readonly Dictionary<string, SymbolState> _states = new();

    public decimal EMAValue(string symbol) => _states.TryGetValue(symbol, out var state) ? state.EMAValue : 0m;
    public bool IsReady(string symbol) => _states.TryGetValue(symbol, out var state) && state.BarCount > _period;

    public EMA(int period, decimal smoothing = 2m, string source = "Close")
    {
        _period = period;
        _smoothing = smoothing;
        _source = source;
        // EMA Multiplier = Smoothing / (Period + 1)
        _multiplier = _smoothing / (_period + 1m);
    }

    public void Update(OhlcvBar bar, string symbol)
    {
        if (!_states.TryGetValue(symbol, out var state))
        {
            state = new SymbolState();
            _states[symbol] = state;
        }

        decimal currentValue = bar[_source] ?? 0m;
        state.BarCount++;

        // During the initial period, we need to gather a Simple Moving Average (SMA) 
        // to use as the base/starting value for the very first EMA calculation.
        if (state.BarCount <= _period)
        {
            state.SumForWarmup += currentValue;
            
            if (state.BarCount == _period)
            {
                state.EMAValue = state.SumForWarmup / _period;
            }
        }
        else
        {
            // O(1) EMA calculation: EMA = (Price * Multiplier) + (Previous EMA * (1 - Multiplier))
            // Optimized formula mathematically identical: 
            // New EMA = Previous EMA + Multiplier * (Current Price - Previous EMA)
            state.EMAValue += _multiplier * (currentValue - state.EMAValue);
        }
    }
}
