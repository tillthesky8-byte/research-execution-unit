using Contracts.Rows;

namespace Modules.Indicators;

public class RSI
{
    private class SymbolState
    {
        public decimal? PreviousClose { get; set; }
        public decimal AverageGain { get; set; }
        public decimal AverageLoss { get; set; }
        public decimal RSIValue { get; set; }
        public int BarCount { get; set; }
        public decimal GainSum { get; set; }
        public decimal LossSum { get; set; }
    }

    private readonly int _period;
    private readonly string _source;
    private readonly Dictionary<string, SymbolState> _states = new();

    public decimal RSIValue(string symbol) => _states.TryGetValue(symbol, out var state) ? state.RSIValue : 0m;
    public bool IsReady(string symbol) => _states.TryGetValue(symbol, out var state) && state.BarCount > _period;

    public RSI(int period, string source = "Close")
    {
        _period = period;
        _source = source;
    }

    public void Update(OhlcvBar bar, string symbol)
    {
        if (!_states.TryGetValue(symbol, out var state))
        {
            state = new SymbolState();
            _states[symbol] = state;
        }

        decimal currentPrice = bar[_source] ?? 0m;

        // First bar: just store the price, no change calculation
        if (state.PreviousClose == null)
        {
            state.PreviousClose = currentPrice;
            state.BarCount = 1;
            return;
        }

        // Calculate the change from previous close
        decimal change = currentPrice - state.PreviousClose.Value;
        decimal gain = change > 0 ? change : 0m;
        decimal loss = change < 0 ? -change : 0m;

        state.BarCount++;
        state.PreviousClose = currentPrice;

        // Warmup phase: accumulate raw sums for the first period
        if (state.BarCount <= _period)
        {
            state.GainSum += gain;
            state.LossSum += loss;

            // Initialize averages after first period
            if (state.BarCount == _period)
            {
                state.AverageGain = state.GainSum / _period;
                state.AverageLoss = state.LossSum / _period;
            }
        }
        else
        {
            // After warmup: apply Wilder's Smoothing
            // New Avg = ((Old Avg * (Period - 1)) + Current Value) / Period
            state.AverageGain = ((state.AverageGain * (_period - 1)) + gain) / _period;
            state.AverageLoss = ((state.AverageLoss * (_period - 1)) + loss) / _period;
        }

        // Calculate RSI after warmup period
        if (state.BarCount > _period)
        {
            if (state.AverageLoss == 0m)
            {
                // If no losses, RSI = 100
                state.RSIValue = 100m;
            }
            else
            {
                decimal rs = state.AverageGain / state.AverageLoss;
                state.RSIValue = 100m - (100m / (1m + rs));
            }
        }
    }
}
