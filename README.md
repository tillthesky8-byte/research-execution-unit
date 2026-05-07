# Research Execution Unit (REU)

**Status:** 🚧 Active Development

REU is a C# algorithmic trading simulation and data pipeline system. 

Currently, this project serves as fundamentally the **backtest unit** for a broader, future research framework. The planned future roadmap for this framework includes extending it with:
- Strategy parameters optimization
- Stress tests and robustness analysis

## Components

- **`REU.App`** - Entry point and CLI/Command definitions (Pipeline/Simulator runners).
- **`REU.Contracts`** - Core domain models, definitions, interfaces, and enums.
- **`REU.Modules`** - Reusable financial indicators and trading strategies.
- **`REU.Pipeline`** - Data pipeline components (Factories, Fusers, Loaders, Writers).
- **`REU.Simulator`** - Backtesting and simulation engine (Execution, Recorders).
