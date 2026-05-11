using Contracts.Rows;

namespace Contracts.Models;

public class MarketCarryState
{
    public Dictionary<string, OhlcvBar> LastBars { get; } = new(StringComparer.OrdinalIgnoreCase);
    public void Update(string symbol, OhlcvBar bar)
    {
        LastBars[symbol] = bar;
    }
    public bool TryGetLastBar(string symbol, out OhlcvBar bar)
    {
        LastBars.TryGetValue(symbol,  out var pulledBar);
        bar = pulledBar ?? new OhlcvBar { Open = 0, High = 0, Low = 0, Close = 0, Volume = 0 };
        return pulledBar != null;
    }
}