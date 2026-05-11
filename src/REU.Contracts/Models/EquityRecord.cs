namespace Contracts.Models;

public record EquityRecord
{
    public long Time { get; init; }
    public decimal Value { get; init; }

    public EquityRecord(EquityPoint equityPoint)
    {
        Time = toUnixTimestamp(equityPoint.Timestamp);
        Value = equityPoint.Equity;
    }
    private long toUnixTimestamp(DateTime dateTime) => new DateTimeOffset(dateTime).ToUnixTimeSeconds();
}