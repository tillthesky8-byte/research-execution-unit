using Contracts.Enums;
using Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using Simulator.Execution.Brokers;
using Simulator.Execution.ComissionModels;
using Simulator.Execution.SlippageModels;

namespace Simulator.Factories;

public static class BrokerFactory
{
    public static IBroker CreateBroker(SlippageModelType slippageModelType, ComissionModelType comissionModelType, ILoggerFactory loggerFactory)
    {
        var slippageModel = slippageModelType switch
        {
            SlippageModelType.Default => new DefaultSlippageModel(),
            _ => throw new ArgumentException($"Unsupported slippage model type: {slippageModelType}")
        };

        var comissionModel = comissionModelType switch
        {
            ComissionModelType.Default => new DefaultComissionModel(),
            _ => throw new ArgumentException($"Unsupported commission model type: {comissionModelType}")
        };

        return new Broker(loggerFactory.CreateLogger<Broker>(), comissionModel, slippageModel);
    }
}
