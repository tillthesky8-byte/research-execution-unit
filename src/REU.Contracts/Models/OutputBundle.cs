using Contracts.Rows;

namespace Contracts.Models;

public sealed record OutputBundle
(
    List<MarketRow>? MarketData = null,
    SimulationResult? SimulationResult = null
);