using Contracts.Models;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface IStrategy
{
    void Initialize(IReadOnlyDictionary<string, string> parameters);
    IEnumerable<OrderRequest> OnTick(MarketContext context, IReadOnlyPortfolio portfolio);
}