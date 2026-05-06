using Contracts.Enums;

namespace Contracts.Models;

public record OrderRequest
(
    string Symbol,
    OrderSide Side,
    OrderType Type,
    decimal Quantity,
    decimal? LimitPrice = null,
    decimal? StopPrice = null
);