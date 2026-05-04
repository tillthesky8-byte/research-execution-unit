# Copilot Instructions — REU Project

## Purpose
REU is a modular trading simulation and research system.  
The goal is to simulate market behavior, generate signals, allocate capital, and evaluate results.

---

## Architecture Overview

The system is split into clear layers:

- REU.Contracts → core models, enums, value objects (no logic tied to IO)
- REU.Modules → pluggable behavior (strategies, indicators, models)
- REU.Pipeline → data flow (loading, alignment, output generation)
- REU.Simulator → simulation runtime (execution loop, broker, portfolio)
- REU.App → CLI entry point and orchestration

---

## Design Principles

1. Separation of concerns
   - Modules must NOT access Pipeline directly
   - Simulator must NOT perform data loading or preprocessing
   - Pipeline prepares data; Simulator consumes it

2. Data flow is one-directional
   MarketContext → Features → Strategy → Signals → Allocator → Orders → Broker

3. No hidden side effects
   - Methods should be deterministic where possible
   - Avoid global mutable state

4. Prefer composition over inheritance

5. Time-series first
   - Data is sequential
   - Avoid random access unless necessary

6. Namespaces shouldn't have REU.* in them.  

7. Dictionary's method TryGetValue should be wrapped in helper methods that return nullable value, with explicit and domain-specific names. For example, instead of `dictionary.TryGetValue(key, out value)`, we should have a method like `TryGetStrategy(string strategyName)` that returns `Strategy?`. This improves readability and encapsulates the dictionary access logic.

---

## What NOT to generate

- Do NOT introduce database logic into Modules
- Do NOT couple strategies to file paths or App
- Do NOT create “god classes” that handle multiple responsibilities
- Do NOT use backward-fill (lookahead bias)

---

## Testing Expectations

- Core logic must be testable without IO
- Use small sample datasets for tests
- Deterministic outputs are required

---

## Summary

This is a simulation system, not a web app.  
Keep logic deterministic, modular, and data-driven.