using Contracts.Models;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface IStrategy
{
    IEnumerable<OrderRequest> OnTick(MarketContext context, IReadOnlyPortfolio portfolio);
}