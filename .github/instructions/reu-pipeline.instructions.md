---
description: "Use when building data pipelines, loaders, transformers, alignment or output logic in REU.Pipeline. Covers data preparation and IO."
applyTo: "src/REU.Pipeline/**/*.cs"
---

# REU.Pipeline Guidelines

REU.Pipeline handles all data loading, alignment, and preprocessing. It depends on REU.Contracts but must NOT access Modules, Simulator, or App directly (unidirectional data flow).

## Core Principles

1. **One-way output to Simulator**
   - Pipeline outputs streams or collections of `MarketContext`
   - Simulator consumes this, never calls back to Pipeline
   - No bidirectional dependency

2. **Temporal alignment**
   - Align all features to market data timestamps
   - Backward-fill is a choice (document any lookahead bias)
   - Validate alignment before output

3. **Lazy evaluation preferred**
   - Use `IEnumerable<T>` for streaming large datasets
   - Materialize only when necessary
   - Reduce memory footprint for long histories

4. **Deterministic output**
   - Same input data → same output every time
   - Document any non-determinism (e.g., sampling)
   - Include seed/random state if applicable

5. **Explicit configuration**
   - Accept file paths, column mappings, and parameters via constructor
   - No hardcoded paths
   - Configuration should be externalized (YAML, JSON)

## Project Structure

```
REU.Pipeline/
├── Loaders/
│   ├── CSVDataLoader.cs
│   ├── SqliteDataLoader.cs
│   └── ...
├── Transformers/
│   ├── ITransformer.cs
│   ├── Fuser.cs
│   ├── NormalizeTransformer.cs
│   └── ...
├── Outputs/
│   ├── IOutput.cs
│   ├── ConsoleOutput.cs
│   └── ...
├── Pipelines/
│   ├── IPipeline.cs
│   ├── DataPipeline.cs
│   └── ...
└── Config/
    └── PipelineConfig.cs
```