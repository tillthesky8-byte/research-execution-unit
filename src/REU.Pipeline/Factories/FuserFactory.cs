using Contracts.Enums;
using Contracts.Interfaces;
using Pipeline.Fusers;
using Microsoft.Extensions.Logging;

namespace Pipeline.Factories;

public static class FuserFactory
{
    public static IFuser CreateFuser(FuserType strategy, ILoggerFactory loggerFactory)
    {
        return strategy switch
        {
            FuserType.LastObservationCarriedForward => new FuserLOCF(loggerFactory.CreateLogger<FuserLOCF>()),
            _ => throw new NotImplementedException($"Fuser type {strategy} is not implemented.")
        };
    }
}