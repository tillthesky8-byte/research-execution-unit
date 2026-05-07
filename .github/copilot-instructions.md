# REU Project Guidelines

## Project Overview
REU is a C# algorithmic trading simulation and data pipeline system. It comprises data loading, strategy processing, simulation, and execution environments using financial time series and market data types.

## Folder Structure
- `configs/` - Configuration templates and YAML files
- `data/` - Raw datasets (CSV/SQLite) and sample market data (e.g., EURUSD, US100, interest rates)
- `src/REU.App/` - Entry point and CLI/Command definitions (Pipeline/Simulator runners)
- `src/REU.Contracts/` - Core domain models, definitions, interfaces, and enums (e.g., IStrategy, ISimulator, Portfolio, TradeRecord)
- `src/REU.Modules/` - Reusable financial indicators and trading strategies
- `src/REU.Pipeline/` - Data pipeline components (Factories, Fusers, Loaders, Writers)
- `src/REU.Simulator/` - Backtesting and simulation engine (Execution, Recorders, Runners)
- `tests/REU.Tests/` - Unit tests for the solution

## Core Development Principles

- **Composition over Inheritance:** Prefer composing objects to share behavior rather than deep inheritance hierarchies.
- **Namespace Structure:** Namespaces DO NOT strictly follow the folder structure. Specifically, do not include the `REU.*` prefix in the namespace naming (for example, avoid `REU.Contracts.` and use a more top-level naming where appropriate according to existing project conventions).
- **Domain Helpers:** Wrap dictionary access or `TryGetValue` methods into semantic helper methods that have clear domain names indicating their purpose (e.g., instead of exposing `TryGetValue`, provide methods like `GetOptionOrDefault` or `TryFetchExecutionModel`).