namespace Contracts.Models;

public sealed record IndexEntry
(
    DateTime RunDate,
    string RunId,
    string StrategyName,
    string[] Symbols,
    DateTime StartDate,
    DateTime EndDate
);