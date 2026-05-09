
using System.Text.Json;
using Contracts.Interfaces;
using Contracts.Models;
using Microsoft.Extensions.Logging;
namespace Writer.Output;
        
public sealed class Manager : IManager
{
    private readonly string _outputDirectory;
    private readonly string _runId;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
    private readonly ILogger<Manager> _logger;

    public Manager(string outputDirectory, string runId, ILogger<Manager> logger)
    {
        _outputDirectory = outputDirectory;
        _runId = runId;
        _logger = logger;
    }


    public void SaveSeries<T>(IEnumerable<T> series, string seriesName)
    {
        CreateOutputDirectoryIfNotExists();

        var filePath = Path.Combine(_outputDirectory, _runId, $"{seriesName}.json");
        var json     = JsonSerializer.Serialize(series, _jsonOptions);

        File.WriteAllText(filePath, json);

        _logger.LogDebug("Saved series {SeriesName} to {FilePath}", seriesName, filePath);

    }


    private void CreateOutputDirectoryIfNotExists()
    {
        var directoryPath = Path.Combine(_outputDirectory, _runId);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            _logger.LogDebug("Created output directory at {DirectoryPath}", directoryPath);
        }
    }
}