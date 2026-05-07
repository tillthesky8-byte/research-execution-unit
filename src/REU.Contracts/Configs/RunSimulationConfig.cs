using Contracts.Enums;

namespace Contracts.Configs;

public class RunSimulationConfig
{
    public decimal InitialCash { get; set; }
    public SlippageModelType SlippageModel { get; set; }
    public ComissionModelType ComissionModel { get; set; }
    public bool IncludeMarketFrame { get; set; } = false;
    public bool IncludeTradeLog { get; set; } = true;
    public bool IncludeEquityCurve { get; set; } = true;
}