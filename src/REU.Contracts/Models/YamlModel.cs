using Contracts.Enums;

namespace Contracts.Models;

public class YamlModel
{
    public string? Name { get; set; }
    public List<string>? Instruments { get; set; }
    public List<string>? Factors { get; set; }
    public Timeframe? Timeframe { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public YamlStrategyDefinition? Strategy { get; set; }
}

public class YamlStrategyDefinition
{
    public StrategyType Type { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
}