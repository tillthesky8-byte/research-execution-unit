using Contracts.Enums;
using Contracts.Models;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface IWriter
{
    public Task WriteFrameAsync(IEnumerable<MarketContext> data);
    public Task WriteTradeLogAsync(IEnumerable<Trade> tradeLog);
    public Task WriteEquityCurveAsync(IEnumerable<EquityPoint> equityCurve);
}