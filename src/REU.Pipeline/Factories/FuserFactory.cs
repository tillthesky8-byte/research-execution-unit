using Contracts.Enums;
using Contracts.Interfaces;
using Pipeline.Fusers;
using Microsoft.Extensions.Logging;

namespace Pipeline.Factories;

public static class FuserFactory
{
    public static IFuser CreateFuser(FuserType type, ILoggerFactory loggerFactory)
    {
        return type switch
        {
            FuserType.LastObservationCarriedForward => new FuserLOCF(loggerFactory.CreateLogger<FuserLOCF>()),
            _ => throw new NotImplementedException($"Fuser type {type} is not implemented.")
        };
    }
}