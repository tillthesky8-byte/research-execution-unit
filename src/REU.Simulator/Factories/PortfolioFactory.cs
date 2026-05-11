using Contracts.Models;
using Microsoft.Extensions.Logging;

namespace Simulator.Factories;

public static class PortfolioFactory
{
    public static Portfolio CreatePortfolio(decimal initialCash, ILogger<Portfolio> logger) => new Portfolio(initialCash, logger);
}