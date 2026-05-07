using System.CommandLine;
using System.CommandLine.Parsing;
using Contracts.Definitions;

namespace App.Options;
public class InstrumentOption : Option<InstrumentDefinition[]>
{
    public InstrumentOption() : base("--instrument", "-i")
    {
        Description = "Defines an instrument to be included in the dataset. Can specify multiple instruments by repeating the option (e.g., --instrument AAPL --instrument MSFT)";
        Arity = ArgumentArity.OneOrMore;
        AllowMultipleArgumentsPerToken = true;
        CustomParser = Parser;
    }

    private InstrumentDefinition[] Parser(ArgumentResult result)
    {
        var values = result.Tokens
            .Select(t => t.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToArray();

        if (values.Length == 0)
            throw new ArgumentException("At least one instrument must be specified using --instrument or -i option.");
        
        var instrumentDefinitions = new List<InstrumentDefinition>();
        foreach(var value in values)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Instrument symbol cannot be empty or whitespace.");
            try
            {
                var instrumentDefinition = ParseInstrumentDefinition(value);
                instrumentDefinitions.Add(instrumentDefinition);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Error parsing instrument definition '{value}': {ex.Message}");
            }
        }
        return instrumentDefinitions.ToArray();
    }

    private InstrumentDefinition ParseInstrumentDefinition(string input)
    {
        return new InstrumentDefinition { Symbol = input.Trim() };
    }
}