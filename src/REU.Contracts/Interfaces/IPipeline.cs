using Contracts.Models;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface IPipeline
{
    public Task<IReadOnlyList<MarketContext>> ExecuteAsync();
    public Task WriteFrameAsync(IReadOnlyList<MarketContext> marketContexts);
    public Task WriteTradeLogAsync(IReadOnlyList<Trade> tradeLog);
    public Task WriteEquityCurveAsync(IReadOnlyList<EquityPoint> equityCurve);
}