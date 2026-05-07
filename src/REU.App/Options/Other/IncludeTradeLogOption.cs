using System.CommandLine;
namespace App.Options;

public sealed class IncludeTradeLogOption : Option<bool?>
{
    public IncludeTradeLogOption() : base("--include-trade-log", "-tl")
    {
        Description = "Include the trade log in the output (if supported by the writer). True by default.";
    }
}