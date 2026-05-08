using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using App.Commands;
using System.Diagnostics.Metrics;
using Contracts.Models;
using Contracts.Definitions;
using Contracts.Enums;
namespace App.Options;
public class YamlSimulationOptions
{
    private readonly IDeserializer _deserializer;
    public InstrumentDefinition[]? Instruments { get; set; }
    public Timeframe? Timeframe { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public FactorDefinition[]? Factors { get; set; }
    public StrategyDefinition? StrategyDefinition { get; set; }

    public YamlSimulationOptions()
    {
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }
    public void LoadFromYaml(string configPath, string configName)
    {
        var filePath = Path.Combine(configPath, $"{configName}.yaml");
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"YAML configuration file not found: {filePath}");
        
        var yamlContent = File.ReadAllText(filePath);
        var config = _deserializer.Deserialize<YamlModel>(yamlContent);

        Instruments = config.Instruments?.Select(i => new InstrumentDefinition { Symbol = i.Trim() }).ToArray();
        Timeframe = config.Timeframe;
        StartDate = config.StartDate;
        EndDate = config.EndDate;
        Factors = config.Factors?.Select(f => new FactorDefinition { Name = f, Timeframe = Contracts.Enums.Timeframe.ANY }).ToArray() ?? Array.Empty<FactorDefinition>();
        
        StrategyDefinition = config.Strategy == null ? null : new StrategyDefinition(config.Strategy.Type, config.Strategy.Parameters ?? new Dictionary<string, string>());

        // Console.WriteLine($"Loaded YAML configuration from {filePath}:");
        // Console.WriteLine($"  Instruments: {string.Join(", ", Instruments?.Select(i => i.Symbol) ?? Array.Empty<string>())}");
        // Console.WriteLine($"  Timeframe: {Timeframe}");
        // Console.WriteLine($"  StartDate: {StartDate}");
        // Console.WriteLine($"  EndDate: {EndDate}");
        // Console.WriteLine($"  Factors: {string.Join(", ", Factors?.Select(f => f.Name) ?? Array.Empty<string>())}");
        // Console.WriteLine($"  Strategy: {StrategyDefinition?.Type} with parameters {string.Join(", ", StrategyDefinition?.Parameters.Select(p => $"{p.Key}={p.Value}") ?? Array.Empty<string>())}");
    }
}