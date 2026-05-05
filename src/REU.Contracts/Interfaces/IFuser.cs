using Contracts.Rows;
namespace Contracts.Interfaces;

public interface IFuser
{
    IReadOnlyList<MarketContext> Fuse
    (
        IReadOnlyDictionary<string, IReadOnlyList<OhlcvRow>> ohlcvData,
        IReadOnlyDictionary<string, IReadOnlyList<FactorRow>> factorData
    );
}