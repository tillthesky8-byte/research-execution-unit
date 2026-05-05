using Contracts.Interfaces;
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
        }
        return Task.CompletedTask;
    }
}