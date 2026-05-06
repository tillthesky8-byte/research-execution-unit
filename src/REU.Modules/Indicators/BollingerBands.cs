using Contracts.Rows;

namespace Modules.Indicators;

public class BollingerBands
{
    private readonly int _period;
    private readonly decimal _stdDevMultiplier;
    private readonly string _source;
    private readonly Dictionary<string, Queue<decimal?>> _priceWindows = new();
    private Dictionary<string, decimal> _upperBands = new();
    private Dictionary<string, decimal> _middleBands = new();
    private Dictionary<string, decimal> _lowerBands = new();

    public decimal UpperBand(string symbol) => _upperBands.GetValueOrDefault(symbol);
    public decimal MiddleBand(string symbol) => _middleBands.GetValueOrDefault(symbol);
    public decimal LowerBand(string symbol) => _lowerBands.GetValueOrDefault(symbol);
    public bool IsReady(string symbol) => _priceWindows.TryGetValue(symbol, out var window) && window.Count == _period;

    public BollingerBands(int period, decimal stdDevMultiplier, string source)
    {
        _period = period;
        _stdDevMultiplier = stdDevMultiplier;
        _source = source;
    }

    public void Update(OhlcvBar bar, string symbol)
    {
        if (!_priceWindows.ContainsKey(symbol))
            _priceWindows[symbol] = new Queue<decimal?>();

        var window = _priceWindows[symbol];
        window.Enqueue(bar[_source]);
        if (window.Count > _period)
            window.Dequeue();

        if (window.Count == _period)
        {
            _middleBands[symbol] = window.Average() ?? 0;
            decimal stdDev = StdDev(window);
            _upperBands[symbol] = _middleBands[symbol] + _stdDevMultiplier * stdDev;
            _lowerBands[symbol] = _middleBands[symbol] - _stdDevMultiplier * stdDev;
        }
    }

    private decimal StdDev(IEnumerable<decimal?> values)
    {
        decimal mean = values.Average() ?? 0;
        decimal variance = values.Average(v => (v - mean) * (v - mean)) ?? 0;
        return (decimal)Math.Sqrt((double)variance);
    }

}
