using System.Text.Json;
using Contracts.Configs;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.Extensions.Logging;
namespace Writer.Indexers;

public sealed class Indexer : IIndexer
{
    private readonly string                 _outputRoot;
    private readonly string                 IndexFileName  = "index.json";
    private readonly string                 ConfigFileName = "config.json";
    private readonly ILogger<Indexer>?      _logger;
    private readonly JsonSerializerOptions _jsonOptions    = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public Indexer(string outputRoot, ILogger<Indexer>? logger)
    {
        _outputRoot = outputRoot;
        _logger     = logger;
    }

    public void RecreateIndex()
    {
        _logger?.LogDebug($"Looking for output directories in {_outputRoot}...");

        var entries = Directory
            .EnumerateDirectories(_outputRoot)
            .Select(BuildEntry)
            .Where(entry => entry is not null)
            .Select(entry => entry!)
            .OrderByDescending(entry => entry.RunDate)
            .ToArray();

        WriteIndexHolder(new IndexHolder(entries));
    }

    private IndexEntry? BuildEntry(string runDir)
    {
        var configPath = Path.Combine(runDir, ConfigFileName);
        if (!File.Exists(configPath))
        {
            _logger?.LogDebug($"Config file not found at {configPath}, skipping entry creation for {runDir}");
            return null;
        }
        var configJson = File.ReadAllText(configPath);
        var runConfig = JsonSerializer.Deserialize<RunConfig>(configJson, _jsonOptions);
        var entry = new IndexEntry
        (
            RunDate:      runConfig?.RunDate ?? DateTime.MinValue,
            RunId:        runConfig?.RunId ?? "unknown",
            StrategyName: runConfig?.SimulatorDefinition?.Strategy.Type.ToString() ?? "unknown",
            Symbols:      runConfig?.PipelineDefinition?.Dataset.Instruments.Select(i => i.Symbol).ToArray() ?? Array.Empty<string>(),
            StartDate:    runConfig?.PipelineDefinition?.Dataset.StartDate ?? DateTime.MinValue,
            EndDate:      runConfig?.PipelineDefinition?.Dataset.EndDate ?? DateTime.MinValue
        );
        return entry;
    }

    private void WriteIndexHolder(IndexHolder index)
    {
        var indexPath = Path.Combine(_outputRoot, IndexFileName);
        var json = JsonSerializer.Serialize(index, _jsonOptions);
        File.WriteAllText(indexPath, json);
        _logger?.LogDebug($"Index written to {indexPath}");
    }
    
}