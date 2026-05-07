using Contracts.Enums;

namespace Contracts.Configs;

public class RunSimulationConfig
{
    public decimal InitialCash { get; set; }
    public SlippageModelType SlippageModel { get; set; }
    public ComissionModelType ComissionModel { get; set; }
}