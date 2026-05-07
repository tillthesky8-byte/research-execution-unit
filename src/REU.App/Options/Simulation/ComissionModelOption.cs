using System.CommandLine;
using System.CommandLine.Parsing;
using Contracts.Enums;

namespace App.Options;

public class ComissionModelOption : Option<ComissionModelType?>
{
    public ComissionModelOption() : base("--comission-model", "-cm")
    {
        Description = "Specifies the comission model to be used in the simulation";
        CustomParser = Parser;
    }

    private ComissionModelType? Parser(ArgumentResult result)
    {
        var value = result.Tokens
            .Select(t => t.Value)
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        if (string.IsNullOrWhiteSpace(value))
            return null; 

        var allowedComissionModels = Enum.GetNames<ComissionModelType>();
        if (!allowedComissionModels.Contains(value, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Invalid comission model '{value}'. Allowed comission models are: {string.Join(", ", allowedComissionModels)}.");

        return Enum.Parse<ComissionModelType>(value, ignoreCase: true);
    }
}