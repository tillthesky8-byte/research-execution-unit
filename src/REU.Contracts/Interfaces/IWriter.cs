using Contracts.Enums;
using Contracts.Models;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface IWriter
{
    public Task WriteFrameAsync(IEnumerable<MarketRow> data, string runId);
    public Task WriteTradeLogAsync(IEnumerable<Trade> tradeLog, string runId);
    public Task WriteEquityCurveAsync(IEnumerable<EquityPoint> equityCurve, string runId);
}