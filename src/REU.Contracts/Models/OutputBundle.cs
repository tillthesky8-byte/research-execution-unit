using Contracts.Rows;
using Contracts.Configs;
namespace Contracts.Models;

public sealed record OutputBundle
(
    List<MarketRow>? MarketData = null,
    SimulationResult? SimulationResult = null,
    RunConfig? RunConfig = null
);