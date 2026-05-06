using Contracts.Enums;
namespace Contracts.Configs;

public record RunPipelineConfig
{  
    public required string ConnectionString { get; init; }
    public required string FilePath { get; init; }
    public required string OutputPath { get; init; }  
    public LoaderType LoaderType { get; init; } = LoaderType.Sqlite;
    public FuserType FuserType { get; init; } = FuserType.LastObservationCarriedForward;
    public WriterType WriterType { get; init; } = WriterType.CsvFile;
}