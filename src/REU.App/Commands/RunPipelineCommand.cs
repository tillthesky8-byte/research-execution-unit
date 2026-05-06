using System.CommandLine;
using App.Options;
using App.Runners;
using Contracts.Configs;
using Contracts.Definitions;
using Contracts.Enums;
using Microsoft.Extensions.Logging;
namespace App.Commands;

public class RunPipelineCommand : Command
{
    public RunPipelineCommand(RunPipelineConfig config, ILoggerFactory loggerFactory) : base("pipeline", "Run the data processing pipeline with the specified configuration.")
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
            // note: introduce mapper 
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

            // note: introduce separate validation layer
            if (instruments == null || instruments.Length == 0)
                throw new ArgumentException("At least one instrument must be specified using --instrument or -i option.");

            if (timeframe == null)
                throw new ArgumentException("A timeframe must be specified using --timeframe or -tf option.");

            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be later than end date.");

            var runId = BuildRunId("pipeline", instruments);
            var outputDirectory = Path.Combine(outputPath, runId);

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
                OutputPath = outputDirectory,
                LoaderType = loader,
                FuserType = fuser,
                WriterType = writer
            };

            var pipelineRunner = new PipelineRunner(loggerFactory);

            await pipelineRunner.RunAsync(pipelineDefinition);
        });
    }

    private static string BuildRunId(string subcommand, IReadOnlyList<InstrumentDefinition> instruments)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var symbols = string.Join("-", instruments.Select(instrument => instrument.Symbol.Trim()));
        return $"{timestamp}_{subcommand}_{symbols}";
    }
}