using Contracts.Enums;
namespace Contracts.Models;

public class Order
{
    public Guid Id { get; } = Guid.NewGuid();
    public required string Symbol { get; init; }
    public required OrderSide Side { get; set; }
    public required OrderType Type { get; set; }
    public required decimal Quantity { get; set; }
    public decimal AverageFillPrice { get; set; }
    public decimal? LimitPrice { get; set; }
    public decimal? StopPrice { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.New;
}