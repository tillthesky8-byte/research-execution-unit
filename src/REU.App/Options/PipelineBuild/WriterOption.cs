using System.CommandLine;
using System.CommandLine.Parsing;
using Contracts.Enums;
namespace App.Options;
public class WriterOption : Option<WriterType?>
{
    public WriterOption() : base("--writer", "-w")
    {
        Description = "Overrides the writer type for the pipeline from appsettings.json.";
        CustomParser = Parser;
    }

    private WriterType? Parser(ArgumentResult result)
    {
        var value = result.Tokens
            .Select(t => t.Value)
            .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        if (string.IsNullOrWhiteSpace(value))
            return null;

        var allowedWriterTypes = Enum.GetNames<WriterType>();
        if (!allowedWriterTypes.Contains(value, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Invalid writer type '{value}'. Allowed writer types are: {string.Join(", ", allowedWriterTypes)}.");

        return Enum.Parse<WriterType>(value, ignoreCase: true);
    }
}