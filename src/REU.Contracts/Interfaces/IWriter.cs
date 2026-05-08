using Contracts.Enums;
using Contracts.Models;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface IWriter
{
    public Task WriteFrameAsync(IEnumerable<MarketContext> data, string runId);
    public Task WriteTradeLogAsync(IEnumerable<Trade> tradeLog, string runId);
    public Task WriteEquityCurveAsync(IEnumerable<EquityPoint> equityCurve, string runId);
}