using Contracts.Rows;

namespace Contracts.Interfaces;

public interface IPipeline
{
    public Task<IReadOnlyList<MarketContext>> ExecuteAsync();
}