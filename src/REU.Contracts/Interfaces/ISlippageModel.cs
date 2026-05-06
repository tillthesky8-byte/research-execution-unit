using Contracts.Models;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface ISlippageModel
{
    decimal Apply(decimal rawPrice,  OrderRequest order, MarketContext marketContext);
}