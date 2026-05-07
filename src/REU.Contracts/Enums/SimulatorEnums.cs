namespace Contracts.Enums;

public enum OrderSide { Buy, Sell }

public enum OrderType { Market, Limit, Stop }

public enum OrderStatus { New, PartiallyFilled, Filled, Canceled, Rejected }

public enum Direction { Long, Short, Flat }

public enum TradeAction { Open, ScaleIn, PartialClose, Close, Reverse }

public enum SlippageModelType { Default }
public enum ComissionModelType { Default }
public enum StrategyType { BBB, BBBV2 }