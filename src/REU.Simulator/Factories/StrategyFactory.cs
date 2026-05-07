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
            StrategyType.BBBV2 => new BBBV2(loggerFactory.CreateLogger<BBBV2>()),
            _ => throw new NotImplementedException($"Strategy type {type} is not implemented.")
        };
    }
}