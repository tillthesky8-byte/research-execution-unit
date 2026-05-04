---
description: "Use when implementing strategies, indicators, or pluggable components in REU.Modules. Covers behavior modules."
applyTo: "src/REU.Modules/**/*.cs"
---

# REU.Modules Guidelines

REU.Modules contains pluggable behavior: indicators, strategies, ML models (later strategies will be broke down into signal extractor and alloc). It depends on REU.Contracts but must NOT access Pipeline, Simulator, or App.

## Core Principles

1. **Depend only on Contracts**
   - Import only `REU.Contracts`
   - Never import from Pipeline, Simulator, or App
   - This ensures modules can be tested in isolation

2. **Composition over inheritance**
   - Use constructor injection to receive dependencies
   - Prefer small, focused interfaces
   - Avoid deep hierarchies

3. **Stateless strategies**
   - Strategies receive `MarketContext` and emit order requests (later signals)
   - Keep state external if needed (managed by Simulator or Pipeline)
   - Pure functions where possible

5. **Immutable outputs**
   - Return `IEnumerable<T>` (lazy evaluation)
   - Or return immutable collections

## Project Structure

```
REU.Modules/
├── Strategies/
│   ├── IStrategy.cs
│   ├── MovingAverageStrategy.cs
│   └── ...
├── Indicators/
│   ├── IIndicator.cs
│   ├── RSIIndicator.cs
│   └── ...
└── Models/
    └── (domain models specific to behavior, not shared types)
```
