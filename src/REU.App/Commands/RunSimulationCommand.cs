using System.CommandLine;
using App.Options;
using Contracts.Configs;
using Contracts.Definitions;
using Contracts.Enums;
using Microsoft.Extensions.Logging;
using App.Runners;
using Contracts.Models;
namespace App.Commands;

public class RunSimulationCommand : Command
{
    private readonly ILogger<RunSimulationCommand> _logger;
    public RunSimulationCommand(AppSettings appSettings, ILoggerFactory loggerFactory) : base("simulation", "Run a backtest simulation with the specified configuration")
    {
        _logger = loggerFactory.CreateLogger<RunSimulationCommand>();
        var opts = new SimulationCommandOptions();

        foreach (var option in opts.GetAllOptions())
            Add(option);

        SetAction(async (context) =>
        {
            //==============================================================================
            // FOUNDATIONAL SETTINGS
            //==============================================================================
            var connectionString   = appSettings.ConnectionString;
            var filePath           = appSettings.SourceFilePath;
            var outputPath         = appSettings.OutputRoot;
            var configPath         = appSettings.ConfigurationRoot;

            var loader             = appSettings.LoaderType;
            var fuser              = appSettings.FuserType;
            var writer             = appSettings.WriterType;
           

            var initialCash        = appSettings.InitialCash; 
            var slippageModel      = appSettings.SlippageModelType; 
            var comissionModel     = appSettings.ComissionModelType; 

            var IncludeOhlcvFrames = appSettings.IncludeOhlcvFrames;
            var includeTradeLog    = appSettings.IncludeTradeLog;
            var includeEquityCurve = appSettings.IncludeEquityCurve;



            //==============================================================================
            // YAML CONFIGURATION LOADING
            //==============================================================================

            var yamlConfigName     = context.GetValue(opts.YamlConfig);

            var yamlOptions        = new YamlSimulationOptions();

            if (!string.IsNullOrEmpty(yamlConfigName))
                yamlOptions.LoadFromYaml(appSettings.ConfigurationRoot, yamlConfigName);

            
            //==============================================================================
            // PARSE COMMAND-LINE OVERRIDES
            //==============================================================================
            var cliInstruments = context.GetValue(opts.Instruments);

            var instruments = (((cliInstruments != null && cliInstruments.Length > 0) ? cliInstruments : yamlOptions.Instruments)
                ?? throw new ArgumentException("At least one instrument must be specified using --instrument or -i option, or in the YAML configuration file."))
                .Cast<InstrumentDefinition>()
                .ToArray();
                
            if (instruments.Length == 0)
                throw new ArgumentException("At least one instrument must be specified using --instrument or -i option, or in the YAML configuration file.");
            
            var timeframe = context.GetValue(opts.Timeframe)     
                ?? yamlOptions.Timeframe 
                ?? throw new ArgumentException("A timeframe must be specified using --timeframe or -tf option, or in the YAML configuration file.");
            
            var startDate = context.GetValue(opts.StartDate)     
                ?? yamlOptions.StartDate 
                ?? DateTime.MinValue;
            
            var endDate = context.GetValue(opts.EndDate)         
                ?? yamlOptions.EndDate 
                ?? DateTime.MaxValue;
            
            var cliFactors = context.GetValue(opts.Factors);
            var factors = (cliFactors != null && cliFactors.Length > 0)
                ? cliFactors
                : (yamlOptions.Factors ?? Array.Empty<FactorDefinition>());
            
            var cliStrategy = context.GetValue(opts.Strategy);
            var strategy = cliStrategy ?? yamlOptions.StrategyDefinition ?? throw new ArgumentException("A strategy must be specified using --strategy or -s option, or in the YAML configuration file.");



            //==============================================================================
            // BUILD CONFIG OBJECTS
            //==============================================================================
            var pipelineDefinition = new PipelineDefinition
            {
                Dataset = new DatasetDefinition
                {
                    Instruments = instruments,
                    Timeframe   = timeframe,
                    StartDate   = startDate,  
                    EndDate     = endDate,
                    Factors     = factors
                },
                Source             = loader == LoaderType.Sqlite ? connectionString : filePath,
                OutputPath         = outputPath,
                IncludeOhlcvFrames = IncludeOhlcvFrames,
                IncludeTradeLog    = includeTradeLog,
                IncludeEquityCurve = includeEquityCurve,
                LoaderType         = loader,
                FuserType          = fuser,
                WriterType         = writer
            };

            var simulatorDefinition = new SimulatorDefinition(
                Strategy       :strategy,
                InitialCash    :initialCash,
                SlippageModel  :slippageModel,
                ComissionModel :comissionModel
            );

            var outputDefinition = new OutputDefinition
            {
                OutputPath         = outputPath,
                IncludeOhlcvFrames = IncludeOhlcvFrames,
                IncludeTradeLog    = includeTradeLog,
                IncludeEquityCurve = includeEquityCurve,
            };
            _logger.LogDebug("OutputDefinition: OutputPath: {OutputPath}, IncludeOhlcvFrames: {IncludeOhlcvFrames}, IncludeTradeLog: {IncludeTradeLog}, IncludeEquityCurve: {IncludeEquityCurve}", outputDefinition.OutputPath, outputDefinition.IncludeOhlcvFrames, outputDefinition.IncludeTradeLog, outputDefinition.IncludeEquityCurve);

            var runConfig = new RunConfig
            {
                PipelineDefinition  = pipelineDefinition,
                SimulatorDefinition = simulatorDefinition,
                OutputDefinition    = outputDefinition
            };

            runConfig.SetRunId();
            runConfig.SetRunDate();

            //==============================================================================
            // RUN SIMULATION
            //==============================================================================

            var simulationRunner = new SimulationRunner(runConfig, loggerFactory);

            await simulationRunner.Run();
        });
    }
}