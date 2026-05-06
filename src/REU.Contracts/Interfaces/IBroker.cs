using Contracts.Models;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface IBroker
{
    void ProcessOrders(MarketContext marketContext, Portfolio portfolio);
    void SubmitOrder(OrderRequest orderRequest, MarketContext marketContext, Portfolio portfolio);
}