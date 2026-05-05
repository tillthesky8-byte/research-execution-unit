using Contracts.Enums;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface IWriter
{
    public Task WriteFrameAsync(IEnumerable<MarketContext> data);
}