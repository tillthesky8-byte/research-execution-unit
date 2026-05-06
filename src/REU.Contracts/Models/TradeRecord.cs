using Contracts.Enums;

namespace Contracts.Models;

public record Trade(
    string Symbol,
    DateTime Timestamp,
    OrderSide Side,
    decimal Quantity,
    decimal Price,
    decimal CommissionPaid,
    TradeAction Action
);
