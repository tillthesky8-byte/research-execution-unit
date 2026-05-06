
namespace Contracts.Models;

public record SimulationResult(
    IReadOnlyList<EquityPoint> EquityCurve,
    IReadOnlyList<Trade> Trades,
    decimal TotalReturn,
    decimal AnnualizedReturn,
    decimal MaxDrawdown,
    decimal SharpeRatio,
    decimal SortinoRatio,
    DateTime StartDate,
    DateTime EndDate

)
{
    public decimal TotalCommission => Trades.Sum(t => t.CommissionPaid);
    public override string ToString() => $"""
        Backtest Summary:
        ────────────────────────────────
        Period:            {StartDate:yyyy-MM-dd} -> {EndDate:yyyy-MM-dd}
        Total Return:      {TotalReturn:P2}
        Annualized Return: {AnnualizedReturn:P2}
        Max Drawdown:      {MaxDrawdown:P2}
        Sharpe Ratio:      {SharpeRatio:F2}
        Sortino Ratio:     {SortinoRatio:F2}
        Total Commission:  {TotalCommission:C}
        Total Trades:      {Trades.Count}
        ────────────────────────────────
        """;
}