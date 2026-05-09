using Microsoft.Extensions.Logging;
using System.CommandLine;
using App.Options;
using App.Runners;
using Contracts.Configs;
using Contracts.Definitions;
using Contracts.Enums;
using Contracts.Models;
namespace App.Commands;

public class RunPipelineCommand : Command
{
    public RunPipelineCommand(AppSettings appSettings, ILoggerFactory loggerFactory) : base("pipeline", "Run the data processing pipeline with the specified configuration.")
    {

        var opts = new PipelineCommandOptions();

        foreach (var option in opts.GetAllOptions())
            Add(option);

        SetAction(async (context) =>
        {
            var instruments      = context.GetValue(opts.Instruments);
            var timeframe        = context.GetValue(opts.Timeframe);
            var startDate        = context.GetValue(opts.StartDate);
            var endDate          = context.GetValue(opts.EndDate);
            var factors          = context.GetValue(opts.Factors);

            var connectionString = appSettings.ConnectionString;
            var filePath         = appSettings.SourceFilePath;
            var outputPath       = appSettings.OutputRoot;

            var loader           = appSettings.LoaderType;
            var fuser            = appSettings.FuserType;
            var writer           = appSettings.WriterType;

            var includeOhlcvFrames  = appSettings.IncludeOhlcvFrames;
            var includeTradeLog     = appSettings.IncludeTradeLog;
            var includeEquityCurve  = appSettings.IncludeEquityCurve;


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
                    Timeframe   = timeframe.Value,
                    StartDate   = startDate ?? DateTime.MinValue,  
                    EndDate     = endDate ?? DateTime.MaxValue,
                    Factors     = factors ?? []
                },
                IncludeEquityCurve = includeEquityCurve,
                IncludeOhlcvFrames = includeOhlcvFrames,
                IncludeTradeLog    = includeTradeLog,
                Source             = loader == LoaderType.Sqlite ? connectionString : filePath,
                OutputPath         = outputPath,
                LoaderType         = loader,
                FuserType          = fuser,
                WriterType         = writer
            };

            var runConfig = new RunConfig
            {
                PipelineDefinition = pipelineDefinition
            };

            var pipelineRunner = new PipelineRunner(runConfig, loggerFactory);

            await pipelineRunner.RunAsync();
        });
    }
}