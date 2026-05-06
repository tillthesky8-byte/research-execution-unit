using Contracts.Enums;
using Contracts.Interfaces;
using Contracts.Models;
using Contracts.Rows;
using Contracts;
using Microsoft.Extensions.Logging;

namespace Simulator.Execution.Brokers;

public class Broker : IBroker
{
    private readonly ILogger<Broker> _logger;
    private readonly IComissionModel _comissionModel;
    private readonly ISlippageModel _slippageModel;
    private readonly List<Order> _pendingOrders = new();

    public Broker(ILogger<Broker> logger, IComissionModel comissionModel, ISlippageModel slippageModel)
    {
        _logger = logger;
        _comissionModel = comissionModel;
        _slippageModel = slippageModel;
    }

    public void SubmitOrder(OrderRequest orderRequest, MarketContext marketContext, Portfolio portfolio)
    {
        var order = new Order
        {
            Timestamp = marketContext.Timestamp,
            Request = orderRequest
        };
        _pendingOrders.Add(order);
        _logger.LogInformation(LogMessages.OrderSubmitted(order.Id, orderRequest.Symbol, orderRequest.Quantity, orderRequest.Type, orderRequest.Side));
    }
    public void ProcessOrders(MarketContext marketContext, Portfolio portfolio)
    {
        var toRemove = new List<Order>();
        foreach (var pending in _pendingOrders)
        {
            if (!marketContext.PriceData.TryGetValue(pending.Request.Symbol, out var symbolBar))
                continue;

            bool triggered = pending.Request.Type switch
            {
                OrderType.Market => true,
                OrderType.Limit => pending.Request.Side == OrderSide.Buy ? symbolBar.Low <= pending.Request.LimitPrice : symbolBar.High >= pending.Request.LimitPrice,
                OrderType.Stop => pending.Request.Side == OrderSide.Buy ? symbolBar.High >= pending.Request.StopPrice : symbolBar.Low <= pending.Request.StopPrice,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (triggered)
                FillOrder(pending, marketContext, portfolio);
                toRemove.Add(pending);
            
        }
        _pendingOrders.RemoveAll(toRemove.Contains);
    }
    private void FillOrder(Order order, MarketContext marketContext, Portfolio portfolio)
    {
        if (!marketContext.PriceData.TryGetValue(order.Request.Symbol, out var symbolBar))
            return;

        decimal rawPrice = order.Request.Type switch
        {
            OrderType.Market => symbolBar.Close,
            OrderType.Limit => order.Request.LimitPrice!.Value,
            OrderType.Stop => symbolBar.Close,
            _ => throw new ArgumentOutOfRangeException()
        };

        decimal fillPrice = _slippageModel.Apply(rawPrice, order.Request, marketContext);
        decimal comission = _comissionModel.CalculateCommission(fillPrice, order.Request.Quantity);

        decimal totalCost = fillPrice * order.Request.Quantity + comission;

        if (order.Request.Side == OrderSide.Buy)
        {
            if (portfolio.Cash < totalCost)
            {
                _logger.LogWarning(LogMessages.NotEnoughCash(order.Id, order.Request.Symbol, totalCost, portfolio.Cash));
                return;
            }

            portfolio.AdjustPosition(order.Request.Symbol, order.Request.Quantity, fillPrice);
            portfolio.UpdateCash(-totalCost);
            _logger.LogInformation(LogMessages.BuyOrderExecuted(order.Id, order.Request.Symbol, order.Request.Quantity, fillPrice, comission, totalCost));
        }
        else
        {
            // margin account will be implemented later, for now we allow short selling without borrowing constraints
            portfolio.AdjustPosition(order.Request.Symbol, -order.Request.Quantity, fillPrice);
            portfolio.UpdateCash(totalCost);
           _logger.LogInformation(LogMessages.SellOrderExecuted(order.Id, order.Request.Symbol, order.Request.Quantity, fillPrice, comission, totalCost));
        }


    }


}
