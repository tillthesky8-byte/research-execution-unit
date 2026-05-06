using Contracts.Models;

namespace Simulator.Factories;

public static class PortfolioFactory
{
    public static Portfolio CreatePortfolio(decimal initialCash) => new Portfolio(initialCash);
}