using Contracts.Models;
using Contracts.Rows;

namespace Contracts.Interfaces;

public interface IBroker
{
    void ProcessOrders(MarketRow MarketRow, Portfolio portfolio);
    void SubmitOrder(OrderRequest orderRequest, MarketRow MarketRow, Portfolio portfolio);
}