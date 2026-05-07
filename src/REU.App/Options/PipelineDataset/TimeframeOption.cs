using System.CommandLine;
using System.CommandLine.Parsing;

using Contracts.Enums;

namespace App.Options;

public class TimeframeOption : Option<Timeframe?>
{
    public TimeframeOption() : base("--timeframe", "-tf")
    {
        Description = "Specifies the timeframe for the dataset (e.g., d, 1h, 15m).";
        CustomParser = Parser;
    }

    private Timeframe? Parser(ArgumentResult result)
    {
        var value = result.Tokens
            .Select(t => t.Value)
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A timeframe value must be specified using --timeframe or -tf option.");

        var allowedTimeframes = Enum.GetNames<Timeframe>();
        if (!allowedTimeframes.Contains(value, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Invalid timeframe '{value}'. Allowed timeframes are: {string.Join(", ", allowedTimeframes)}.");

        return Enum.Parse<Timeframe>(value, ignoreCase: true);
    }
}