using System.CommandLine;
namespace App.Options;

public sealed class IncludeMarketFrameOption : Option<bool?>
{
    public IncludeMarketFrameOption() : base("--include-market-frame", "-mf")
    {
        Description = "Include the market frame in the output (if supported by the writer). False by default.";
    }
}