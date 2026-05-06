using Contracts.Models;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface ISimulator
{
    SimulationResult Run(IEnumerable<MarketContext> marketData); 
}