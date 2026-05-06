using System.CommandLine;
using System.CommandLine.Parsing;
using Contracts.Enums;
namespace App.Options;
public class LoaderOption : Option<LoaderType?>
{
    public LoaderOption() : base("--loader", "-l")
    {
        Description = "Overrides the loader type for the pipeline from appsettings.json.";
        CustomParser = Parser;
    }

    private LoaderType? Parser(ArgumentResult result)
    {
        var value = result.Tokens
            .Select(t => t.Value)
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        if (string.IsNullOrWhiteSpace(value))
            return null; 

        var allowedLoaderTypes = Enum.GetNames<LoaderType>();
        if (!allowedLoaderTypes.Contains(value, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Invalid loader type '{value}'. Allowed loader types are: {string.Join(", ", allowedLoaderTypes)}.");

        return Enum.Parse<LoaderType>(value, ignoreCase: true);
    }
}