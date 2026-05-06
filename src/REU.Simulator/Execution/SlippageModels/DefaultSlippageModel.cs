using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;

namespace Simulator.Execution.SlippageModels;

public class DefaultSlippageModel : ISlippageModel
{
    public decimal Apply(decimal rawPrice, OrderRequest order, MarketContext marketContext) =>
        rawPrice; // No slippage applied, return the raw price
}