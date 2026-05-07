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
    public RunSimulationCommand(RunSimulationConfig simulationConfig, RunPipelineConfig pipelineConfig, ILoggerFactory loggerFactory) : base("simulation", "Run a backtest simulation with the specified configuration")
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

        var strategyOption = new StrategyOption();
        var initialCashOption = new InitialCashOption();
        var slippageModelOption = new SlippageModelOption();
        var comissionModelOption = new ComissionModelOption();

        var includeMarketFrameOption = new IncludeMarketFrameOption();
        var includeTradeLogOption = new IncludeTradeLogOption();
        var includeEquityCurveOption = new IncludeEquityCurveOption();


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

        Add(strategyOption);
        Add(initialCashOption);
        Add(slippageModelOption);
        Add(comissionModelOption);

        Add(includeMarketFrameOption);
        Add(includeTradeLogOption);
        Add(includeEquityCurveOption);


        SetAction(async (context) =>
        {
            var instruments = context.GetValue(instrumentOption);
            var timeframe = context.GetValue(timeframeOption);
            var startDate = context.GetValue(startDateOption);
            var endDate = context.GetValue(endDateOption);
            var factors = context.GetValue(factorOption);
            
            var connectionString = context.GetValue(connectionStringOption) ?? pipelineConfig.ConnectionString;
            var filePath = context.GetValue(filePathOption) ?? pipelineConfig.FilePath;
            var outputPath = context.GetValue(outputPathOption) ?? pipelineConfig.OutputPath;
            
            var loader = context.GetValue(loaderOption) ?? pipelineConfig.LoaderType;
            var fuser = context.GetValue(fuserOption) ?? pipelineConfig.FuserType;
            var writer = context.GetValue(writerOption) ?? pipelineConfig.WriterType;
           
            var strategy = context.GetValue(strategyOption) ?? throw new ArgumentException("Strategy definition is required to run the simulation. Please specify it using --strategy or -strat option.");
            var initialCash = context.GetValue(initialCashOption) ?? simulationConfig.InitialCash; 
            var slippageModel = context.GetValue(slippageModelOption) ?? simulationConfig.SlippageModel; 
            var comissionModel = context.GetValue(comissionModelOption) ?? simulationConfig.ComissionModel; 

            var includeMarketFrame = context.GetValue(includeMarketFrameOption) ?? simulationConfig.IncludeMarketFrame;
            var includeTradeLog = context.GetValue(includeTradeLogOption) ?? simulationConfig.IncludeTradeLog;
            var includeEquityCurve = context.GetValue(includeEquityCurveOption) ?? simulationConfig.IncludeEquityCurve;
            
            if (instruments == null || instruments.Length == 0)
                throw new ArgumentException("At least one instrument must be specified using --instrument or -i option.");

            if (timeframe == null)
                throw new ArgumentException("A timeframe must be specified using --timeframe or -tf option.");

            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be later than end date.");

            var runId = Program.BuildRunId("simulation", instruments);
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

            var simulatorDefinition = new SimulatorDefinition(
                Strategy:       strategy,
                InitialCash:    initialCash,
                SlippageModel:  slippageModel,
                ComissionModel: comissionModel
            );

            var simulationRunner = new SimulatorRunner(loggerFactory);

            await simulationRunner.Run(simulatorDefinition, pipelineDefinition, includeMarketFrame: includeMarketFrame, includeTradeLog: includeTradeLog, includeEquityCurve: includeEquityCurve);
        });
    }
}