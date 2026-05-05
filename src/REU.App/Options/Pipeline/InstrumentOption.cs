using System.CommandLine;
using System.CommandLine.Parsing;

namespace App.Options;
public class InstrumentOption : Option<string[]>
{
    public InstrumentOption() : base("--instrument", "-i")
    {
        Description = "Defines an instrument to be included in the dataset. Can specify multiple instruments by repeating the option (e.g., --instrument AAPL --instrument MSFT)";
        Arity = ArgumentArity.OneOrMore;
        AllowMultipleArgumentsPerToken = true;
        CustomParser = Parser;
    }

    private string[] Parser(ArgumentResult result)
    {
        var values = result.Tokens
            .Select(t => t.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToArray();

        if (values.Length == 0)
            throw new ArgumentException("At least one instrument must be specified using --instrument or -i option.");

        return values;
    }
}