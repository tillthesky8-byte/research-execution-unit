
using Contracts.Enums;

namespace Contracts;
public static class LogMessages
{
    public static string BuyOrderExecuted(Guid orderId, string symbol, decimal quantity, decimal price, decimal comission, decimal totalCost)
    {
        return $"{ConsoleColors.Cyan}{orderId}{ConsoleColors.Reset} {ConsoleColors.Green}BUY Order EXECUTED{ConsoleColors.Reset}. SYM: {ConsoleColors.ValueColor}{symbol}{ConsoleColors.Reset} QTY: {ConsoleColors.ValueColor}{quantity}{ConsoleColors.Reset} PRICE: {ConsoleColors.ValueColor}{price}{ConsoleColors.Reset} COMSN: {ConsoleColors.ValueColor}{comission}{ConsoleColors.Reset} TOTAL: {ConsoleColors.ValueColor}{totalCost}{ConsoleColors.Reset}";
    }

    public static string SellOrderExecuted(Guid orderId, string symbol, decimal quantity, decimal price, decimal comission, decimal totalCost)
    {
        return $"{ConsoleColors.Cyan}{orderId}{ConsoleColors.Reset} {ConsoleColors.Red}SELL Order EXECUTED{ConsoleColors.Reset}. SYM: {ConsoleColors.ValueColor}{symbol}{ConsoleColors.Reset} QTY: {ConsoleColors.ValueColor}{quantity}{ConsoleColors.Reset} PRICE: {ConsoleColors.ValueColor}{price}{ConsoleColors.Reset} COMSN: {ConsoleColors.ValueColor}{comission}{ConsoleColors.Reset} TOTAL: {ConsoleColors.ValueColor}{totalCost}{ConsoleColors.Reset}";
    }

    public static string NotEnoughCash(Guid orderId, string symbol, decimal totalCost, decimal cashAvailable)
    {
        return $"{ConsoleColors.Cyan}{orderId}{ConsoleColors.Reset} {ConsoleColors.Red}BUY Order REJECTED{ConsoleColors.Reset}. Not enough cash to execute order for {ConsoleColors.ValueColor}{symbol}{ConsoleColors.Reset}. Required: {ConsoleColors.ValueColor}{totalCost}{ConsoleColors.Reset}, Available: {ConsoleColors.ValueColor}{cashAvailable}{ConsoleColors.Reset}";
    }

    public static string OrderSubmitted(Guid orderId, string symbol, decimal quantity, OrderType type, OrderSide side)
    {
        return $"{ConsoleColors.Cyan}{orderId}{ConsoleColors.Reset} {side} {type} Order SUBMITTED for {ConsoleColors.ValueColor}{symbol}{ConsoleColors.Reset} QTY: {ConsoleColors.ValueColor}{quantity}{ConsoleColors.Reset}";
    }

    public static string OnNewTickPortfolioOverview(decimal equity, decimal cash) =>
        $"New Tick - Portfolio Overview: {ConsoleColors.ValueColor}Equity: {equity}{ConsoleColors.Reset}, {ConsoleColors.ValueColor}Cash: {cash}{ConsoleColors.Reset}";

    public static string OnNewTickPendingOrders(int count) =>
        $"New Tick - Pending Orders: {ConsoleColors.ValueColor}{count}{ConsoleColors.Reset}";
        
}