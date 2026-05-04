---
description: "Use when implementing the simulation runtime, broker logic, portfolio tracking, or execution loop in REU.Simulator. Covers simulation mechanics."
applyTo: "src/REU.Simulator/**/*.cs"
---

# REU.Simulator Guidelines

REU.Simulator executes the simulation loop: consuming market data, applying strategies, executing orders, and tracking results. It depends on REU.Contracts but must NOT perform data loading (Pipeline handles that).

## Core Principles

1. **Consume, don't produce data**
   - Simulator receives `IEnumerable<MarketContext>` from Pipeline
   - Never loads files or queries databases directly
   - All preprocessing is complete before simulation starts

2. **Execution loop is deterministic**
   - For each `MarketContext`, apply strategies → generate orders → execute via broker
   - Same input → same portfolio state and equity curve
   - No randomness unless explicitly seeded

3. **No side effects in simulation**
   - Simulator doesn't call Pipeline, Modules, or App
   - Results are collected, returned, or streamed
   - Caller decides what to do with results

4. **Portfolio state is mutable**
   - Track positions, cash, equity over time
   - Update atomically per time-step
   - Maintain order history for audit

5. **Broker is the execution layer**
   - Translates `OrderCommand[]` to fills
   - Applies slippage, commissions, constraints
   - Returns `Fill[]` or execution summary

## Project Structure

```
REU.Simulator/
├── Core/
│   ├── ISimulator.cs
│   ├── SimulationEngine.cs
│   └── SimulationResult.cs
├── Broker/
│   ├── IBroker.cs
│   ├── SimpleBroker.cs
│   └── ExecutionContext.cs
├── Portfolio/
│   ├── Portfolio.cs
│   ├── Position.cs
│   └── PortfolioMetrics.cs
└── Events/
    ├── ExecutionEvent.cs
    └── TradeLog.cs
```

