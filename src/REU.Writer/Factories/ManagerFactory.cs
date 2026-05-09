using Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using Contracts.Enums;
using Writer.Output;

namespace Writer.Factories;

public static class ManagerFactory
{
    public static IManager CreateManager(ManagerType managerType, string outputRoot, string runId, ILoggerFactory loggerFactory)
    {
        return managerType switch
        {
            ManagerType.Default => new Manager(outputRoot, runId, loggerFactory.CreateLogger<Manager>()),
            _ => throw new NotImplementedException($"Manager type {managerType} is not implemented.")
        };
    }
}