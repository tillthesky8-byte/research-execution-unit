using Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using Contracts.Enums;
using Writer.Indexers;

namespace Writer.Factories;

public static class IndexerFactory
{
    public static IIndexer CreateIndexer(IndexerType indexerType, string outputRoot, ILoggerFactory loggerFactory)
    {
        return indexerType switch
        {
            IndexerType.Default => new Indexer(outputRoot, loggerFactory.CreateLogger<Indexer>()),
            _ => throw new NotImplementedException($"Indexer type {indexerType} is not implemented.")
        };
    }
}