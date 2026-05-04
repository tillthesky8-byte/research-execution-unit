
# REU — Research & Execution Unit

REU is a modular trading simulation and research framework designed for building, testing, and evaluating strategies on historical data.

---

## What it does

- Loads market data (OHLCV + features)
- Prepares aligned datasets through a pipeline
- Runs deterministic simulations
- Generates trade logs and equity curves
- Exports results for analysis (e.g. Python)

---

## Core Concepts

### MarketContext
A single time-step of market data, including:
- OHLCV
- derived features

---

### Strategy
Generates signals based on market state.

Output:
StrategySignal[]

---

### Allocator
Transforms signals into executable orders.

Output:
OrderCommand[]

---

### Simulator
Runs the execution loop:
- processes market data
- applies strategies
- executes orders via broker
- updates portfolio

---

## Project Structure

src/ 
├── REU.Contracts 
├── REU.Pipeline 
├── REU.Simulator 
├── REU.Modules
└── REU.App  
data/ 
├── raw/ 
├── processed/ 
└── samples/  
outputs/ 
└── runs/
configs/
└── templates/

---

## Data Flow

Raw Data → Pipeline → MarketContext → Strategy → Orders → Simulation Result

---

## Design Goals

- Deterministic simulation
- Clear separation of concerns
- High flexibility for experimentation
- Efficient handling of large time-series datasets

---

## Running the system

1. Prepare dataset via Pipeline
2. Run simulation via CLI
3. Analyze outputs (Python recommended)

---

## Notes

- Simulator does NOT perform data preprocessing
- All temporal alignment is done in Pipeline
- Python is used for visualization and analysis

---

## Status

Work in progress — focused on architecture and extensibility over completeness.