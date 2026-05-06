using Contracts.Enums;

namespace Contracts.Models;

public record OrderRequest
(
    string Symbol,
    OrderSide Side,
    decimal Quantity,
    decimal Price
);