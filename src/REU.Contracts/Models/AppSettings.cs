using Contracts.Enums;
using Microsoft.Extensions.Configuration;

namespace Contracts.Models;

public sealed class AppSettings
{

    public string             ConnectionString { get; init; }
    public string             SourceFilePath { get; init; }
    public string             ConfigurationRoot { get; init; }
    public string             OutputRoot { get; init; }

    public LoaderType         LoaderType { get; init; }
    public FuserType          FuserType { get; init; }
    public WriterType         WriterType { get; init; }

    public decimal            InitialCash { get; init; }
    public SlippageModelType  SlippageModelType { get; init; }
    public ComissionModelType ComissionModelType { get; init; }

    public bool               IncludeOhlcvFrames { get; init; }
    public bool               IncludeTradeLog { get; init; }
    public bool               IncludeEquityCurve { get; init; }
    public ManagerType        ManagerType { get; init; }
    public IndexerType        IndexerType { get; init; }


    public AppSettings(ConfigurationManager configuration)
    {
        ConnectionString    = configuration.GetConnectionString("Sqlite")                               ?? throw new InvalidOperationException("Connection string 'Sqlite' not found in configuration.");
        SourceFilePath      = configuration["Paths:SourceFilePath"]                                     ?? throw new InvalidOperationException("Missing configuration value 'Paths:SourceFilePath'.");
        ConfigurationRoot   = configuration["Paths:ConfigurationRoot"]                                  ?? throw new InvalidOperationException("Missing configuration value 'Paths:ConfigurationRoot'.");
        OutputRoot          = configuration["Paths:OutputRoot"]                                         ?? throw new InvalidOperationException("Missing configuration value 'Paths:OutputRoot'.");
        
        LoaderType          = Enum.Parse<LoaderType>(configuration["Pipeline:Loader"]                   ?? throw new InvalidOperationException("Missing configuration value 'Pipeline:Loader'."));
        FuserType           = Enum.Parse<FuserType>(configuration["Pipeline:Fuser"]                     ?? throw new InvalidOperationException("Missing configuration value 'Pipeline:Fuser'."));
        WriterType          = Enum.Parse<WriterType>(configuration["Pipeline:Writer"]                   ?? throw new InvalidOperationException("Missing configuration value 'Pipeline:Writer'."));
        
        InitialCash         = decimal.Parse(configuration["Simulation:InitialCash"]                     ?? throw new InvalidOperationException("Missing configuration value 'Simulation:InitialCash'."));      
        SlippageModelType   = Enum.Parse<SlippageModelType>(configuration["Simulation:SlippageModel"]   ?? throw new InvalidOperationException("Missing configuration value 'Simulation:SlippageModel'."));
        ComissionModelType  = Enum.Parse<ComissionModelType>(configuration["Simulation:ComissionModel"] ?? throw new InvalidOperationException("Missing configuration value 'Simulation:ComissionModel'."));
        
        IncludeOhlcvFrames  = bool.Parse(configuration["Writer:IncludeOhlcvFrames"]                     ?? throw new InvalidOperationException("Missing configuration value 'Writer:IncludeOhlcvFrames'."));
        IncludeTradeLog     = bool.Parse(configuration["Writer:IncludeTradeLog"]                        ?? throw new InvalidOperationException("Missing configuration value 'Writer:IncludeTradeLog'."));
        IncludeEquityCurve  = bool.Parse(configuration["Writer:IncludeEquityCurve"]                     ?? throw new InvalidOperationException("Missing configuration value 'Writer:IncludeEquityCurve'."));
        ManagerType         = Enum.Parse<ManagerType>(configuration["Writer:Manager"]                   ?? throw new InvalidOperationException("Missing configuration value 'Writer:Manager'."));
        IndexerType         = Enum.Parse<IndexerType>(configuration["Writer:Indexer"]                   ?? throw new InvalidOperationException("Missing configuration value 'Writer:Indexer'."));
    }
}