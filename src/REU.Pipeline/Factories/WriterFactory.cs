using Contracts.Enums;
using Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using Pipeline.Writers;

namespace Pipeline.Factories;

public static class WriterFactory
{
    public static IWriter CreateWriter(WriterType type, string path, ILoggerFactory loggerFactory)
    {
        return type switch
        {
            WriterType.CsvFile => new CsvFileWriter(path, loggerFactory.CreateLogger<CsvFileWriter>()),
            WriterType.ParquetFile => throw new NotImplementedException("Parquet file writer is not implemented yet."),
            WriterType.JsonFile => throw new NotImplementedException("Json file writer is not implemented yet."),
            WriterType.Console => new ConsoleWriter(),
            _ => throw new NotImplementedException($"Writer type {type} is not implemented.")
        };
    }
}
