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

        return new SimulationResult
        (
            EquityCurve:      _equityCurve.AsReadOnly(),
            Trades:           _tradeLog.AsReadOnly(),
            TotalReturn:      totalReturn,
            AnnualizedReturn: annualizedReturn,
            MaxDrawdown:      _maxDrawdown,
            StartDate:        _equityCurve.First().Timestamp,
            EndDate:          _equityCurve.Last().Timestamp
        );
    }
}