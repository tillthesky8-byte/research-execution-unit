using Contracts.Rows;
namespace Contracts.Interfaces;

public interface IFuser
{
    IReadOnlyList<MarketRow> Fuse
    (
        IReadOnlyDictionary<string, IReadOnlyList<OhlcvRow>> ohlcvData,
        IReadOnlyDictionary<string, IReadOnlyList<FactorRow>> factorData
    );
}