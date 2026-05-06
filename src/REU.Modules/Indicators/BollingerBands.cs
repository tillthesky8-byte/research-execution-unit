using Contracts.Rows;

namespace Modules.Indicators;

public class BollingerBands
{
    private readonly int _period;
    private readonly decimal _stdDevMultiplier;
    private readonly string _source;
    private readonly Queue<decimal?> _priceWindow = new();
    private decimal _middleBand;
    private decimal _upperBand;
    private decimal _lowerBand;
    public decimal MiddleBand => _middleBand;
    public decimal UpperBand => _upperBand;
    public decimal LowerBand => _lowerBand;
    public bool IsReady => _priceWindow.Count == _period;

    public BollingerBands(int period = 20, decimal stdDevMultiplier = 2m, string source = "Close")
    {
        _period = period;
        _stdDevMultiplier = stdDevMultiplier;
        _source = source;
    }

    public void Update(OhlcvBar bar)
    {
        _priceWindow.Enqueue(bar[_source]);
        if (_priceWindow.Count > _period)
            _priceWindow.Dequeue();

        if (_priceWindow.Count == _period)
        {
            _middleBand = _priceWindow.Average() ?? 0;
            decimal stdDev = StdDev(_priceWindow);
            _upperBand = _middleBand + _stdDevMultiplier * stdDev;
            _lowerBand = _middleBand - _stdDevMultiplier * stdDev;
        }
    }

    private decimal StdDev(IEnumerable<decimal?> values)
    {
        decimal mean = values.Average() ?? 0;
        decimal variance = values.Average(v => (v - mean) * (v - mean)) ?? 0;
        return (decimal)Math.Sqrt((double)variance);
    }

}
