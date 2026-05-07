using System.CommandLine;
using System.CommandLine.Parsing;

using Contracts.Definitions;
using Contracts.Enums;

namespace App.Options;

public class FactorOption : Option<FactorDefinition[]>
{
    public FactorOption() : base("--factor", "-fa")
    {
        Description = "Defines a external factor to be included in the dataset. Format: name:timeframe (e.g., sentiment:d, volatility:1h).";
        Arity = ArgumentArity.OneOrMore;
        AllowMultipleArgumentsPerToken = true;
        CustomParser = Parser;
    }

    private FactorDefinition[] Parser(ArgumentResult result)
    {
        var values = result.Tokens
             .Select(t => t.Value)
             .Where(v => !string.IsNullOrWhiteSpace(v))
             .ToArray();

        var allowedTimeframes = Enum.GetNames<Timeframe>();
        var factorDefinitions = new List<FactorDefinition>();
        foreach (var value in values)
        {
            try
            {
                var factorDefinition = ParseFactorDefinition(value, allowedTimeframes);
                factorDefinitions.Add(factorDefinition);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Error parsing factor definition '{value}': {ex.Message}");
            }
        }

        return factorDefinitions.ToArray();
    }

    private FactorDefinition ParseFactorDefinition(string input, string[] allowedTimeframes)
    {
        var parts = input.Split(':');
        if (string.IsNullOrWhiteSpace(parts[0]))
            throw new ArgumentException($"Invalid factor definition '{input}'. Expected format: name or name:timeframe (e.g., sentiment:d).");

        var name = parts[0].Trim();
        if (parts.Length == 1) return new FactorDefinition { Name = name, Timeframe = Timeframe.ANY };
        var timeframeStr = parts[1].Trim();

        if (!allowedTimeframes.Contains(timeframeStr, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Invalid timeframe '{timeframeStr}' in factor definition '{input}'. Allowed timeframes are: {string.Join(", ", allowedTimeframes)}.");

        var timeframe = Enum.Parse<Timeframe>(timeframeStr, ignoreCase: true);
        return new FactorDefinition { Name = name, Timeframe = timeframe };
    }
}