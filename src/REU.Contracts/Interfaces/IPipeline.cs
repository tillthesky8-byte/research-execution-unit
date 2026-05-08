using Contracts.Models;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface IPipeline
{
    public Task<IReadOnlyList<MarketContext>> ExecuteAsync();
    public Task WriteFrameAsync(IReadOnlyList<MarketContext> marketContexts, string runId);
    public Task WriteTradeLogAsync(IReadOnlyList<Trade> tradeLog, string runId);
    public Task WriteEquityCurveAsync(IReadOnlyList<EquityPoint> equityCurve, string runId);
}