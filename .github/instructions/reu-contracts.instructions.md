---
description: "Use when adding or modifying types, records, enums in REU.Contracts. Covers models without business logic."
applyTo: "src/REU.Contracts/**/*.cs"
---

# REU.Contracts Guidelines

REU.Contracts is the foundational layer containing all core models and interfaces. It has **zero dependencies** on other layers.

## Core Principles

1. **No logic, only shape**
   - Keep classes and records as data containers
   - Interfaces define contracts, not behavior implementation
   - Enums represent finite states

2. **No dependencies outside Contracts**
   - Never import from Modules, Pipeline, Simulator, or App
   - This ensures Contracts remains the foundation for all layers

3. **Nullable-aware design**
   - Use `#nullable enable` consistently
   - Mark optional properties with `?`
   - Default to non-nullable unless reason to allow null

4. **Record types for immutable data**
   - Use `record` for value objects (e.g., `MarketContext`, `StrategySignal`)
   - Use `class` only for stateful domain objects with identity

5. **Explicit constructors**
   - Avoid positional records unless all fields are essential
   - Use named properties for clarity

## Project Structure
```
REU.Contracts/
├── Models/
│   ├── MarketContext.cs
│   ├── StrategySignal.cs
│   └── ...
├── Interfaces/
│   ├── IStrategy.cs
│   ├── IBroker.cs
│   └── ...
└── Enums/
    ├── OrderType.cs
    ├── PositionSide.cs
    └── ...

```