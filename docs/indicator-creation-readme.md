# Indicator Creation Prompt Guide

To get the most out of the **Indicator Architect** agent when generating high-performance indicators for the REU framework, it is crucial to provide a structured prompt. This ensures the agent understands the exact mathematical behavior without needing to guess logic.

## Best Practices

1. **Be specific about the mathematics**: If the indicator is standard (like MACD or RSI), declare it. If it includes custom smoothing or specialized constraints, explicitly state the formula.
2. **Define the inputs and outputs**: What does it consume (`Close`, `High`, `Low`, Volume)? What values does it output?
3. **Mention the warmup period**: Define what conditions satisfy `IsReady()`.
4. **State parameter requirements**: Clearly list the initialization parameters (e.g., `int period, decimal smoothFactor`).

---

## The Ideal Prompt Template

When interacting with the agent, use the following structure:

```markdown
@Indicator Architect Create a [Indicator Name] indicator.

**Parameters**:
- [List parameter 1, e.g., int period]
- [List parameter 2, e.g., decimal multiplier]

**Inputs / Tracking**:
- Needs to track [e.g., Close price, True Range, Volume].
- Uses a period of [X].

**Outputs**:
- [List the distinct output lines, e.g., MainLine, SignalLine, Histogram].

**Formula / Math Details**:
- [Provide the step-by-step mathematical formula or plain language description]
- [Provide how the rolling calculation should work, if known]

**Warmup (IsReady)**:
- True when [Condition].
```

---

## Example Prompt

**Example: Relative Strength Index (RSI)**

> @Indicator Architect Create an RSI (Relative Strength Index) indicator.
> 
> **Parameters**:
> - `int period`
> - `string source` (default to Close)
> 
> **Inputs / Tracking**:
> - Needs to track the positive changes (gains) and negative changes (losses) across consecutive bars.
> 
> **Outputs**:
> - `RSIValue(string symbol)` returning 0 to 100.
> 
> **Formula / Math Details**:
> - Use standard Wilder's Smoothing for the running averages: `New Avg = ((Old Avg * (Period - 1)) + Current Value) / Period`.
> - Calculate `RS = AverageGain / AverageLoss`.
> - If AverageLoss is 0, RSI is 100. Otherwise, `RSI = 100 - (100 / (1 + RS))`.
> 
> **Warmup**:
> - True when we have processed `period` amount of bars.
