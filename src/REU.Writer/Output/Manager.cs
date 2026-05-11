
using System.Text.Json;
using Contracts.Interfaces;
using Contracts.Models;
using Contracts;
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

        _logger.LogDebug("{background}{foreground}{FilePath}{reset} Saved series {SeriesName}", ConsoleColors.PathBackgroundColor, ConsoleColors.PathForegroundColor, filePath, ConsoleColors.Reset, seriesName);

    }

    public void SaveObject<T>(T obj, string objectName)
    {
        CreateOutputDirectoryIfNotExists();

        var filePath = Path.Combine(_outputDirectory, _runId, $"{objectName}.json");
        var json     = JsonSerializer.Serialize(obj, _jsonOptions);

        File.WriteAllText(filePath, json);

        _logger.LogDebug("{background}{foreground}{FilePath}{reset} Saved object {ObjectName}", ConsoleColors.PathBackgroundColor, ConsoleColors.PathForegroundColor, filePath, ConsoleColors.Reset, objectName);
    }


    private void CreateOutputDirectoryIfNotExists()
    {
        var directoryPath = Path.Combine(_outputDirectory, _runId);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            _logger.LogDebug("{background}{foreground}{DirectoryPath}{reset} Created output directory", ConsoleColors.PathBackgroundColor, ConsoleColors.PathForegroundColor, directoryPath, ConsoleColors.Reset);
        }
    }
}