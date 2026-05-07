using Contracts.Rows;

namespace Modules.Indicators;

public class BollingerBands
{
    private class SymbolState
    {
        public Queue<decimal> MiddleWindow { get; } = new();
        public Queue<decimal> SideWindow { get; } = new();
        public decimal MiddleSum { get; set; }
        public decimal MiddleSumOfSquares { get; set; }
        public decimal SideSum { get; set; }
        public decimal SideSumOfSquares { get; set; }
        public decimal UpperBand { get; set; }
        public decimal MiddleBand { get; set; }
        public decimal LowerBand { get; set; }
    }
    private readonly int _middlePeriod;
    private readonly int _sidePeriod;
    private readonly decimal _stdDevMultiplier;
    private readonly string _source;

    private readonly Dictionary<string, SymbolState> _states = new();

    public decimal UpperBand(string symbol) => _states.TryGetValue(symbol, out var state) ? state.UpperBand : 0;
    public decimal MiddleBand(string symbol) => _states.TryGetValue(symbol, out var state) ? state.MiddleBand : 0;
    public decimal LowerBand(string symbol) => _states.TryGetValue(symbol, out var state) ? state.LowerBand : 0;
    public bool IsReady(string symbol) => _states.TryGetValue(symbol, out var state) && state.MiddleWindow.Count == _middlePeriod && state.SideWindow.Count == _sidePeriod;

    public BollingerBands(int middlePeriod, int sidePeriod, decimal stdDevMultiplier, string source)
    {
        _middlePeriod = middlePeriod;
        _sidePeriod = sidePeriod;
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

        // Update middle band state
        state.MiddleWindow.Enqueue(value);
        state.MiddleSum += value;
        state.MiddleSumOfSquares += value * value;

        if (state.MiddleWindow.Count > _middlePeriod)
        {
            decimal old = state.MiddleWindow.Dequeue();
            state.MiddleSum -= old;
            state.MiddleSumOfSquares -= old * old;
        }

        // Update side band state
        state.SideWindow.Enqueue(value);
        state.SideSum += value;
        state.SideSumOfSquares += value * value;

        if (state.SideWindow.Count > _sidePeriod)
        {
            decimal old = state.SideWindow.Dequeue();
            state.SideSum -= old;
            state.SideSumOfSquares -= old * old;
        }

        if (state.MiddleWindow.Count == _middlePeriod && state.SideWindow.Count == _sidePeriod)
        {
            state.MiddleBand = state.MiddleSum / _middlePeriod;

            decimal sideVariance = (state.SideSumOfSquares / _sidePeriod) - ((state.SideSum / _sidePeriod) * (state.SideSum / _sidePeriod));
            decimal sideStdDev = (decimal)Math.Sqrt((double)sideVariance);

            state.UpperBand = state.MiddleBand + _stdDevMultiplier * sideStdDev;
            state.LowerBand = state.MiddleBand - _stdDevMultiplier * sideStdDev;
        }
    }
}