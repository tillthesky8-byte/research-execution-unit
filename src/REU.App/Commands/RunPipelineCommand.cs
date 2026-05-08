using System.CommandLine;
using App.Options;
using App.Runners;
using Contracts.Configs;
using Contracts.Definitions;
using Contracts.Enums;
using Microsoft.Extensions.Logging;
using App;
namespace App.Commands;

public class RunPipelineCommand : Command
{
    public RunPipelineCommand(RunPipelineConfig config, ILoggerFactory loggerFactory) : base("pipeline", "Run the data processing pipeline with the specified configuration.")
    {

        var opts = new PipelineCommandOptions();

        foreach (var option in opts.GetAllOptions())
            Add(option);

        SetAction(async (context) =>
        {
            var instruments = context.GetValue(opts.Instruments);
            var timeframe = context.GetValue(opts.Timeframe);
            var startDate = context.GetValue(opts.StartDate);
            var endDate = context.GetValue(opts.EndDate);
            var factors = context.GetValue(opts.Factors);

            var connectionString = config.ConnectionString;
            var filePath = config.FilePath;
            var outputPath = config.OutputPath;

            var loader = config.LoaderType;
            var fuser = config.FuserType;
            var writer = config.WriterType;

            // note: introduce separate validation layer
            if (instruments == null || instruments.Length == 0)
                throw new ArgumentException("At least one instrument must be specified using --instrument or -i option.");

            if (timeframe == null)
                throw new ArgumentException("A timeframe must be specified using --timeframe or -tf option.");

            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be later than end date.");

            var runId = Program.BuildRunId("pipeline", instruments);
            var outputDirectory = Path.Combine(outputPath, runId);

            var pipelineDefinition = new PipelineDefinition
            {
                Dataset = new DatasetDefinition
                {
                    Instruments = instruments,
                    Timeframe = timeframe.Value,
                    StartDate = startDate ?? DateTime.MinValue,  
                    EndDate = endDate ?? DateTime.MaxValue,
                    Factors = factors ?? Array.Empty<FactorDefinition>()
                },
                IncludeEquityCurve = false,
                IncludeMarketFrame = true,
                IncludeTradeLog = false,
                Source = loader == LoaderType.Sqlite ? connectionString : filePath,
                OutputPath = outputDirectory,
                LoaderType = loader,
                FuserType = fuser,
                WriterType = writer
            };

            var pipelineRunner = new PipelineRunner(loggerFactory);

            await pipelineRunner.RunAsync(pipelineDefinition);
        });
    }
}