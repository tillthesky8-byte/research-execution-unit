using System.CommandLine;
using System.CommandLine.Parsing;

using Contracts.Definitions;
using Contracts.Enums;

namespace App.Options;

public class StrategyOption : Option<StrategyDefinition>
{
    public StrategyOption() : base("--strategy", "-strat")
    {
        Description = "Defines the trading strategy to be used in the simulation. Specify the strategy name followed by any parameters (e.g., --strategy meanReversion:lookback=20,threshold=0.05).";
        Arity = ArgumentArity.ExactlyOne;
        CustomParser = Parser;
    }

    private StrategyDefinition Parser(ArgumentResult result)
    {
        var value = result.Tokens
            .Select(t => t.Value)
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Strategy definition must be specified using --strategy or -strat option.");

        try
        {
            return ParseStrategyDefinition(value);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Error parsing strategy definition '{value}': {ex.Message}");
        }
    }

    private StrategyDefinition ParseStrategyDefinition(string input)
    {
        // expected format: -strat strategyName:param1=value1,param2=value2
        var parts = input.Split(':', 2);
        if (parts.Length == 0 || string.IsNullOrWhiteSpace(parts[0]))
            throw new ArgumentException("Strategy name is required in the strategy definition.");

        var strategyName = parts[0].Trim();
        var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (parts.Length > 1)
        {
            var paramPairs = parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in paramPairs)
            {
                var kv = pair.Split('=', 2);
                if (kv.Length != 2 || string.IsNullOrWhiteSpace(kv[0]) || string.IsNullOrWhiteSpace(kv[1]))
                    throw new ArgumentException($"Invalid parameter format '{pair}'. Expected format is key=value.");

                parameters[kv[0].Trim()] = kv[1].Trim();
            }
        }
        return new StrategyDefinition(Enum.Parse<StrategyType>(strategyName, true), parameters);
    }
}