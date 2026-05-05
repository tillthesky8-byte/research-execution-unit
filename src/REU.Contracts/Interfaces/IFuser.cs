using Contracts.Rows;
namespace Contracts.Interfaces;

public interface IFueser
{
    IReadOnlyList<MarketContext> Fuse
    (
        IReadOnlyDictionary<string, IReadOnlyList<OhlcvRow>> ohlcvData,
        IReadOnlyDictionary<string, IReadOnlyList<FactorRow>> factorData
    );
}