using Contracts.Rows;
namespace Contracts.Interfaces;

public interface IFueser
{
    IReadOnlyList<MarketContext> Fuse(IReadOnlyList<OhlcvRow> ohlcvRows, IReadOnlyList<FactorRow> factorRows);
}