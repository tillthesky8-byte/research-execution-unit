using Contracts.Interfaces;
using Contracts.Models;
namespace Simulator.Recorders;
public class Recorder : IRecorder
{
    private readonly List<EquityPoint> _equityCurve = [];  
    private readonly List<Trade> _tradeLog = [];  
    private decimal _peakEquity = decimal.MinValue;
    private decimal _maxDrawdown = 0;

    public void Record(DateTime timestamp, decimal equity)
    {
        _equityCurve.Add(new EquityPoint(timestamp, equity));

        if (equity > _peakEquity)
            _peakEquity = equity;

        else
        {
            var drawdown = (_peakEquity - equity) / _peakEquity;

            if (drawdown > _maxDrawdown)
                _maxDrawdown = drawdown;
        }
    }

    public void AppendTrades(IEnumerable<Trade> trades) =>
        _tradeLog.AddRange(trades);


    public SimulationResult BuildResult()
    {
        if (_equityCurve.Count < 2)
            throw new InvalidOperationException("At least two equity points are required to calculate total return.");
        

        decimal initialEquity = _equityCurve.First().Equity;
        decimal finalEquity = _equityCurve.Last().Equity;
        TimeSpan duration = _equityCurve.Last().Timestamp - _equityCurve.First().Timestamp;


        var totalReturn = (finalEquity / initialEquity) - 1;

        double years = duration.TotalDays / 365.25;
        var annualizedReturn = (decimal)Math.Pow((double)(finalEquity / initialEquity), 1 / years) - 1;


        var sharpeRatio = ComputeSharp();
        var sortinoRatio = ComputeSortino();

        return new SimulationResult
        (
            EquityCurve:      _equityCurve.AsReadOnly(),
            Trades:           _tradeLog.AsReadOnly(),
            TotalReturn:      totalReturn,
            AnnualizedReturn: annualizedReturn,
            MaxDrawdown:      _maxDrawdown,
            SharpeRatio:      sharpeRatio,
            SortinoRatio:     sortinoRatio,
            StartDate:        _equityCurve.First().Timestamp,
            EndDate:          _equityCurve.Last().Timestamp
        );
    }

    private decimal ComputeSharp(decimal riskFreeRate = 0.0m, int periodsPerYear = 252)
    {
        if (_equityCurve.Count < 2)
            return 0;

        var returns = _equityCurve
            .Zip(_equityCurve.Skip(1), (prev, next) => (next.Equity / prev.Equity) - 1)
            .ToArray();

        decimal mean = returns.Average();
        decimal stdDev = (decimal)Math.Sqrt(returns.Average(r => Math.Pow((double)(r - mean), 2)));
        if (stdDev == 0) return 0;

        return (mean - riskFreeRate / periodsPerYear) / stdDev * (decimal)Math.Sqrt(periodsPerYear);
    }

    private decimal ComputeSortino(decimal riskFreeRate = 0.0m, int periodsPerYear = 252)
    {
        if (_equityCurve.Count < 2)
            return 0;

        var returns = _equityCurve
            .Zip(_equityCurve.Skip(1), (prev, next) => (next.Equity / prev.Equity) - 1)
            .ToArray();

        decimal mean = returns.Average();
        decimal downsideStdDev = (decimal)Math.Sqrt(returns.Where(r => r < riskFreeRate / periodsPerYear).Average(r => Math.Pow((double)(r - riskFreeRate / periodsPerYear), 2)));
        if (downsideStdDev == 0) return 0;

        return (mean - riskFreeRate / periodsPerYear) / downsideStdDev * (decimal)Math.Sqrt(periodsPerYear);
    }
}