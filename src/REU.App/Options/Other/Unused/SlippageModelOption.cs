using System.CommandLine;
using System.CommandLine.Parsing;
using Contracts.Enums;

namespace App.Options;

public class SlippageModelOption : Option<SlippageModelType?>
{
    public SlippageModelOption() : base("--slippage-model", "-sm")
    {
        Description = "Specifies the slippage model to be used in the simulation.";
        CustomParser = Parser;
    }

    private SlippageModelType? Parser(ArgumentResult result)
    {
        var value = result.Tokens
            .Select(t => t.Value)
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        if (string.IsNullOrWhiteSpace(value))
            return null; 

        var allowedSlippageModels = Enum.GetNames<SlippageModelType>();
        if (!allowedSlippageModels.Contains(value, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Invalid slippage model '{value}'. Allowed slippage models are: {string.Join(", ", allowedSlippageModels)}.");

        return Enum.Parse<SlippageModelType>(value, ignoreCase: true);
    }
}