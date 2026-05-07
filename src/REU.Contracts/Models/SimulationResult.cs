
namespace Contracts.Models;

public record SimulationResult(
    IReadOnlyList<EquityPoint> EquityCurve,
    IReadOnlyList<Trade> Trades,
    decimal TotalReturn,
    decimal AnnualizedReturn,
    decimal MaxDrawdown,
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
        Total Commission:  {TotalCommission:C}
        Total Trades:      {Trades.Count}
        ────────────────────────────────
        """;
}