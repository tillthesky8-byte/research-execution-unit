using System.CommandLine;
namespace App.Options;
public class SimulationCommandOptions
{
    public InstrumentOption Instruments { get; }
    public TimeframeOption Timeframe { get; }
    public StartDateOption StartDate { get; }
    public EndDateOption EndDate { get; }
    public FactorOption Factors { get; }

    public StrategyOption Strategy { get; }

    public YamlConfigNameOption YamlConfig { get; }

    public SimulationCommandOptions()
    {
        Instruments = new();
        Timeframe = new();
        StartDate = new();
        EndDate = new();
        Factors = new();
        Strategy = new();
        YamlConfig = new();
    }

    public Option[] GetAllOptions()
    {
        return
        [
            Instruments, Timeframe, StartDate, EndDate, Factors,
            Strategy, YamlConfig
        ];
    }
}