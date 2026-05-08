using System.CommandLine;
namespace App.Options;
public class PipelineCommandOptions
{
    public InstrumentOption Instruments { get; }
    public TimeframeOption Timeframe { get; }
    public StartDateOption StartDate { get; }
    public EndDateOption EndDate { get; }
    public FactorOption Factors { get; }

    public PipelineCommandOptions()
    {
        Instruments = new();
        Timeframe = new();
        StartDate = new();
        EndDate = new();
        Factors = new();


    }

    public Option[] GetAllOptions()
    {
        return
        [
            Instruments, Timeframe, StartDate, EndDate, Factors
        ];
    }
}