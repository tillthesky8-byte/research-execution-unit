using Contracts.Enums;
using Contracts.Interfaces;

namespace Contracts.Definitions;

public sealed record SimulatorDefinition(
    StrategyDefinition Strategy,
    SlippageModelType SlippageModel,
    ComissionModelType ComissionModel,
    decimal InitialCash
);