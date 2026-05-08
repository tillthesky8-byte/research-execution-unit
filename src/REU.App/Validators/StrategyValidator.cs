using Contracts.Definitions;

namespace App.Validators;

public static class StrategyValidator
{
    public static bool ValidateStrategy(StrategyDefinition? strategy)
    {
        if (strategy == null)
            throw new ArgumentException("A strategy must be specified using --strategy or -strat option.");

        return true;
    }
}