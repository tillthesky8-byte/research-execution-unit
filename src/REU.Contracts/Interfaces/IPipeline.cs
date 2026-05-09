using Contracts.Models;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface IPipeline
{
    public Task<IReadOnlyList<MarketRow>> ExecuteAsync();
    public Task WriteFrameAsync(IReadOnlyList<MarketRow> MarketRows, string runId);
    public Task WriteTradeLogAsync(IReadOnlyList<Trade> tradeLog, string runId);
    public Task WriteEquityCurveAsync(IReadOnlyList<EquityPoint> equityCurve, string runId);
}