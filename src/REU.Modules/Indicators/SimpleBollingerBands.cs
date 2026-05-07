using Contracts.Rows;

namespace Modules.Indicators;

public class SimpleBollingerBands
{
    private class SymbolState
    {
        public Queue<decimal> Window { get; } = new();
        public decimal Sum { get; set; }
        public decimal SumOfSquares { get; set; }
        public decimal UpperBand { get; set; }
        public decimal MiddleBand { get; set; }
        public decimal LowerBand { get; set; }
    }

    private readonly int _period;
    private readonly decimal _stdDevMultiplier;
    private readonly string _source;
    private readonly Dictionary<string, SymbolState> _states = new();

    public decimal UpperBand(string symbol) => _states.TryGetValue(symbol, out var state) ? state.UpperBand : 0;
    public decimal MiddleBand(string symbol) => _states.TryGetValue(symbol, out var state) ? state.MiddleBand : 0;
    public decimal LowerBand(string symbol) => _states.TryGetValue(symbol, out var state) ? state.LowerBand : 0;
    public bool IsReady(string symbol) => _states.TryGetValue(symbol, out var state) && state.Window.Count == _period;

    public SimpleBollingerBands(int period, decimal stdDevMultiplier, string source)
    {
        _period = period;
        _stdDevMultiplier = stdDevMultiplier;
        _source = source;
    }

    public void Update(OhlcvBar bar, string symbol)
    {
        if (!_states.TryGetValue(symbol, out var state))
        {
            state = new SymbolState();
            _states[symbol] = state;
        }

        // Safely extract the decimal value
        decimal value = bar[_source] ?? 0m;
        
        state.Window.Enqueue(value);
        state.Sum += value;
        state.SumOfSquares += value * value;

        if (state.Window.Count > _period)
        {
            decimal old = state.Window.Dequeue();
            state.Sum -= old;
            state.SumOfSquares -= old * old;
        }

        if (state.Window.Count == _period)
        {
            state.MiddleBand = state.Sum / _period;
            
            // Variance = E[X^2] - (E[X])^2
            // Math.Max prevents negative square roots from floating-point accumulation errors
            decimal variance = Math.Max(0m, (state.SumOfSquares / _period) - (state.MiddleBand * state.MiddleBand));
            decimal stdDev = (decimal)Math.Sqrt((double)variance);

            state.UpperBand = state.MiddleBand + _stdDevMultiplier * stdDev;
            state.LowerBand = state.MiddleBand - _stdDevMultiplier * stdDev;
        }
    }
}