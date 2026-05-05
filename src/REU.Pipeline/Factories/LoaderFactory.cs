using Contracts.Enums;
using Contracts.Interfaces;
using Pipeline.Loaders;
using Microsoft.Extensions.Logging;

namespace Pipeline.Factories;

public static class LoaderFactory
{
    public static ILoader CreateLoader(LoaderType loaderType, string connectionString, ILoggerFactory loggerFactory)
    {
        return loaderType switch
        {
            LoaderType.Sqlite => new SqliteLoader(connectionString, loggerFactory.CreateLogger<SqliteLoader>()),
            LoaderType.Csv => new CsvLoader(connectionString, loggerFactory.CreateLogger<CsvLoader>()),
            _ => throw new NotImplementedException($"Loader type {loaderType} is not implemented.")
        };
    }
}