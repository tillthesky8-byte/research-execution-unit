---
description: "Use when building the CLI entry point, orchestration logic, or user-facing features in REU.App. Covers application layer."
applyTo: "src/REU.App/**/*.cs"
---

# REU.App Guidelines

REU.App is the CLI orchestration layer. It wires up Pipeline → Simulator → Modules, parses user input, and handles output. It depends on all other layers.

## Core Principles

1. **Thin orchestration layer**
   - App's job is to connect layers, not implement business logic
   - Configuration parsing and validation here

2. **Command-line first**
   - Use `System.CommandLine` for clean argument parsing
   - Subcommands for different workflows ("run" which is pipeline and simulator, "pipeline", "simulate")
   - Options with sensible defaults

3. **External configuration**
   - If command-line input specifies a config file, load it (YAML/JSON)
   - Config should cover all parameters for pipeline, simulator, strategy
   - Paths and parameters configurable, not hardcoded
   - Support environment variable overrides

4. **Clear user feedback**
   - Log progress and results clearly
   - Return meaningful exit codes
   - Output structured data (JSON) for automation

5. **Error handling at boundary**
   - Catch errors from all layers
   - Transform technical errors into user-friendly messages
   - Log full stack traces for debugging

## Project Structure

```
REU.App/
├── Program.cs
├── appsettings.json
│
├── Commands/
│   ├── RunCommand.cs
│   ├── PipelineCommand.cs
│   └── SimulateCommand.cs
├── Options/
│   ├── PipelineDefinition/
│   │   ├── DatasetOptions.cs
│   │   ├── PipelineOptions.cs
│   │   └── OutputOptions.cs
│   ├── SimulationDefinitionOptions.cs
│   └── ... (later)
```
