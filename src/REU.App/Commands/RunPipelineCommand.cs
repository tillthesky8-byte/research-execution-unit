using System.CommandLine;
using App.Options;
namespace App.Commands;

public class RunPipelineCommand : Command
{
    public RunPipelineCommand() : base("pipeline", "Run the data processing pipeline with the specified configuration.")
    {

        var instrumentOption = new InstrumentOption();
        var timeframeOption = new TimeframeOption();
        var startDateOption = new StartDateOption();
        var endDateOption = new EndDateOption();
        var factorOption = new FactorOption();

        Add(instrumentOption);
        Add(timeframeOption);
        Add(startDateOption);
        Add(endDateOption);
        Add(factorOption);

        SetAction(async (context) =>
        {
            var instruments = context.GetValue(instrumentOption);
            var timeframe = context.GetValue(timeframeOption);
            var startDate = context.GetValue(startDateOption);
            var endDate = context.GetValue(endDateOption);
            var factors = context.GetValue(factorOption);

            if (instruments == null || instruments.Length == 0)
                throw new ArgumentException("At least one instrument must be specified using --instrument or -i option.");

            if (timeframe == null)
                throw new ArgumentException("A timeframe must be specified using --timeframe or -tf option.");

            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be later than end date.");

            Console.WriteLine("Instruments:");
            foreach (var instrument in instruments!)
            {
                Console.WriteLine($" - {instrument} {timeframe}");
            }
            Console.WriteLine($"Start Date: {startDate:yyyy-MM-dd}");
            Console.WriteLine($"End Date: {endDate:yyyy-MM-dd}");

            Console.WriteLine("Factors:");
            foreach (var factor in factors!)
            {
                Console.WriteLine($" - {factor.Name}:{factor.Timeframe}");
            }
            await Task.CompletedTask;
        });
    }
}