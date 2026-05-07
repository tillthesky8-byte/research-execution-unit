using System.CommandLine;
using System.CommandLine.Parsing;
using Contracts.Enums;
namespace App.Options;
public class FuserOption : Option<FuserType?>
{
    public FuserOption() : base("--fuser", "-f")
    {
        Description = "Overrides the fuser type for the pipeline from appsettings.json.";
        CustomParser = Parser;
    }

    private FuserType? Parser(ArgumentResult result)
    {
        var value = result.Tokens
            .Select(t => t.Value)
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        if (string.IsNullOrWhiteSpace(value))
            return null;

        var allowedFuserTypes = Enum.GetNames<FuserType>();
        if (!allowedFuserTypes.Contains(value, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Invalid fuser type '{value}'. Allowed fuser types are: {string.Join(", ", allowedFuserTypes)}.");

        return Enum.Parse<FuserType>(value, ignoreCase: true);
    }
}