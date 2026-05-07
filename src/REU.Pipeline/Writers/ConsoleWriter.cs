using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;

namespace Pipeline.Writers;
public class ConsoleWriter : IWriter
{
    public Task WriteFrameAsync(IEnumerable<MarketContext> data)
    {
        foreach (var context in data)
        {
            Console.Write($"Timestamp: {context.Timestamp:o} | ");
            foreach (var priceData in context.PriceData)
            {
                Console.Write($"Symbol: {priceData.Key} | O: {priceData.Value.Open} H: {priceData.Value.High} L: {priceData.Value.Low} C: {priceData.Value.Close} V: {priceData.Value.Volume} | ");
            }
            foreach (var factorData in context.FactorData)            {
                Console.Write($"{factorData.Key}: {factorData.Value} | ");
            }
            Console.WriteLine();
        }
        return Task.CompletedTask;
    }

    public Task WriteTradeLogAsync(IEnumerable<Trade> tradeLog)
    {
        foreach (var trade in tradeLog)
        {
            Console.WriteLine($"Timestamp: {trade.Timestamp:o} | Symbol: {trade.Symbol} | Side: {trade.Side} | Action: {trade.Action} | Quantity: {trade.Quantity} | Price: {trade.Price}");
        }
        return Task.CompletedTask;
    }

    public Task WriteEquityCurveAsync(IEnumerable<EquityPoint> equityCurve)
    {
        foreach (var point in equityCurve)
        {
            Console.WriteLine($"Timestamp: {point.Timestamp:o} | Equity: {point.Equity}");
        }
        return Task.CompletedTask;
    }
}