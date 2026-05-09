using System.CommandLine;
using App.Options;
using Contracts.Configs;
using Contracts.Definitions;
using Contracts.Enums;
using Microsoft.Extensions.Logging;
using App.Runners;
namespace App.Commands;

public class RunSimulationCommand : Command
{
    private readonly ILogger<RunSimulationCommand> _logger;
    public RunSimulationCommand(RunSimulationConfig simulationConfig, RunPipelineConfig pipelineConfig, ILoggerFactory loggerFactory) : base("simulation", "Run a backtest simulation with the specified configuration")
    {
        _logger = loggerFactory.CreateLogger<RunSimulationCommand>();
        var opts = new SimulationCommandOptions();

        foreach (var option in opts.GetAllOptions())
            Add(option);

        SetAction(async (context) =>
        {
            var connectionString = pipelineConfig.ConnectionString;
            var filePath = pipelineConfig.FilePath;
            var outputPath = pipelineConfig.OutputPath;

            var loader = pipelineConfig.LoaderType;
            var fuser = pipelineConfig.FuserType;
            var writer = pipelineConfig.WriterType;
           

            var initialCash = simulationConfig.InitialCash; 
            var slippageModel = simulationConfig.SlippageModel; 
            var comissionModel = simulationConfig.ComissionModel; 

            var includeMarketFrame = simulationConfig.IncludeMarketFrame;
            var includeTradeLog = simulationConfig.IncludeTradeLog;
            var includeEquityCurve = simulationConfig.IncludeEquityCurve;
            var configPath = pipelineConfig.ConfigPath;

            var yamlConfigName = context.GetValue(opts.YamlConfig);

            var yamlOptions = new YamlSimulationOptions();

            if (!string.IsNullOrEmpty(yamlConfigName))
                yamlOptions.LoadFromYaml(pipelineConfig.ConfigPath, yamlConfigName);
            
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
                IncludeMarketFrame = includeMarketFrame,
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
                OutputPath = outputPath,
                IncludeOhlcvFrames = includeMarketFrame,
                IncludeTradeLog = includeTradeLog,
                IncludeEquityCurve = includeEquityCurve,
            };
            _logger.LogDebug("OutputDefinition: OutputPath: {OutputPath}, IncludeOhlcvFrames: {IncludeOhlcvFrames}, IncludeTradeLog: {IncludeTradeLog}, IncludeEquityCurve: {IncludeEquityCurve}", outputDefinition.OutputPath, outputDefinition.IncludeOhlcvFrames, outputDefinition.IncludeTradeLog, outputDefinition.IncludeEquityCurve);

            var runConfig = new RunConfig
            {
                PipelineDefinition  = pipelineDefinition,
                SimulatorDefinition = simulatorDefinition,
                OutputDefinition    = outputDefinition
            };


            var simulationRunner = new SimulationRunner(runConfig, loggerFactory);

            await simulationRunner.Run();
        });
    }
}