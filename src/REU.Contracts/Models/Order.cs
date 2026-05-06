using Contracts.Enums;
namespace Contracts.Models;

public class Order
{
    public Guid Id { get; } = Guid.NewGuid();
    public required DateTime Timestamp { get; set; }
    public required OrderRequest Request { get; set; }

}