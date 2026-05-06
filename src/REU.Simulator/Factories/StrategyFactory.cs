using Contracts.Enums;
using Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using Modules.Strategies;

namespace Simulator.Factories;

public static class StrategyFactory
{
    public static IStrategy CreateStrategy(StrategyType type, ILoggerFactory loggerFactory)
    {
        return type switch
        {
            StrategyType.BBB => new BBB(loggerFactory.CreateLogger<BBB>()),
            _ => throw new NotImplementedException($"Strategy type {type} is not implemented.")
        };
    }
}