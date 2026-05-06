using System.CommandLine;
using App.Options;
using Contracts.Configs;
using Contracts.Definitions;
using Contracts.Enums;
using Microsoft.Data.Sqlite;
using CsvHelper;
namespace App.Commands;

public class RunPipelineCommand : Command
{
    public RunPipelineCommand(RunPipelineConfig config) : base("pipeline", "Run the data processing pipeline with the specified configuration.")
    {

        var instrumentOption = new InstrumentOption();
        var timeframeOption = new TimeframeOption();
        var startDateOption = new StartDateOption();
        var endDateOption = new EndDateOption();
        var factorOption = new FactorOption();

        var connectionStringOption = new ConnectionStringOption();
        var filePathOption = new FilePathOption();
        var outputPathOption = new OutputPathOption();
        
        var loaderOption = new LoaderOption();
        var fuserOption = new FuserOption();
        var writerOption = new WriterOption();

        Add(instrumentOption);
        Add(timeframeOption);
        Add(startDateOption);
        Add(endDateOption);
        Add(factorOption);

        Add(connectionStringOption);
        Add(filePathOption);
        Add(outputPathOption);

        Add(loaderOption);
        Add(fuserOption);
        Add(writerOption);

        SetAction(async (context) =>
        {
            var instruments = context.GetValue(instrumentOption);
            var timeframe = context.GetValue(timeframeOption);
            var startDate = context.GetValue(startDateOption);
            var endDate = context.GetValue(endDateOption);
            var factors = context.GetValue(factorOption);

            var connectionString = context.GetValue(connectionStringOption) ?? config.ConnectionString;
            var filePath = context.GetValue(filePathOption) ?? config.FilePath;
            var outputPath = context.GetValue(outputPathOption) ?? config.OutputPath;

            var loader = context.GetValue(loaderOption) ?? config.LoaderType;
            var fuser = context.GetValue(fuserOption) ?? config.FuserType;
            var writer = context.GetValue(writerOption) ?? config.WriterType;


            if (instruments == null || instruments.Length == 0)
                throw new ArgumentException("At least one instrument must be specified using --instrument or -i option.");

            if (timeframe == null)
                throw new ArgumentException("A timeframe must be specified using --timeframe or -tf option.");

            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be later than end date.");

            var pipelineDefinition = new PipelineDefinition
            {
                Dataset = new DatasetDefinition
                {
                    Instruments = instruments,
                    Timeframe = timeframe.Value,
                    StartDate = startDate,  
                    EndDate = endDate,
                    Factors = factors ?? Array.Empty<FactorDefinition>()
                },
                Source = loader == LoaderType.Sqlite ? connectionString : filePath,
                OutputPath = outputPath,
                LoaderType = loader,
                FuserType = fuser,
                WriterType = writer
            };

            System.Console.WriteLine("Pipeline configuration:");
            System.Console.WriteLine($"Instruments: {string.Join(", ", pipelineDefinition.Dataset.Instruments.Select(i => i.Symbol))}");
            System.Console.WriteLine($"Timeframe: {pipelineDefinition.Dataset.Timeframe}");
            System.Console.WriteLine($"Start Date: {pipelineDefinition.Dataset.StartDate:yyyy-MM-dd}");
            System.Console.WriteLine($"End Date: {pipelineDefinition.Dataset.EndDate:yyyy-MM-dd}");
            System.Console.WriteLine($"Factors: {string.Join(", ", pipelineDefinition.Dataset.Factors.Select(f => f.Name))}");
            System.Console.WriteLine($"Source: {pipelineDefinition.Source}");
            System.Console.WriteLine($"Output Path: {pipelineDefinition.OutputPath}");
            System.Console.WriteLine($"Loader Type: {pipelineDefinition.LoaderType}");
            System.Console.WriteLine($"Fuser Type: {pipelineDefinition.FuserType}");
            System.Console.WriteLine($"Writer Type: {pipelineDefinition.WriterType}");

            //testing sqlite connection
            if (pipelineDefinition.LoaderType == LoaderType.Sqlite)
            {
                try
                {
                    using var connection = new SqliteConnection(pipelineDefinition.Source);
                    await connection.OpenAsync();
                    var query = "SELECT name FROM sqlite_master WHERE type='table' LIMIT 1;";
                    using var command = new SqliteCommand(query, connection);
                    var result = await command.ExecuteScalarAsync();
                    if (result != null)
                        System.Console.WriteLine($"Successfully connected to SQLite database. First table name: {result}");
                
                    else
                        System.Console.WriteLine("Successfully connected to SQLite database, but no tables were found.");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Failed to connect to SQLite database: {ex.Message}");
                }
            }

            //testing file path
            if (pipelineDefinition.LoaderType == LoaderType.Csv)
            {
                var fileName = "{0}_{1}_{2}_{3}.csv";
                 fileName = string.Format(fileName, pipelineDefinition.Dataset.Instruments[0].Symbol,
                    pipelineDefinition.Dataset.Timeframe.ToString().ToLower(),
                    pipelineDefinition.Dataset.StartDate.ToString("yyyyMMdd"),
                    pipelineDefinition.Dataset.EndDate.ToString("yyyyMMdd"));
                 var testFilePath = Path.Combine(pipelineDefinition.Source, fileName);

                if (File.Exists(testFilePath))                
                    System.Console.WriteLine($"Successfully found the data file at: {testFilePath}");
            
                else
                    System.Console.WriteLine($"Data file not found at: {testFilePath}");
            }
            await Task.CompletedTask;
        });
    }
}