namespace Contracts.Rows;

public record FactorRow
{
    public DateTime Timestamp { get; init; }
    public decimal Value { get; init; }
}