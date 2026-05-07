using System.CommandLine;
namespace App.Options;

public sealed class IncludeEquityCurveOption : Option<bool?>
{
    public IncludeEquityCurveOption() : base("--include-equity-curve", "-ec")
    {
        Description = "Include the equity curve in the output (if supported by the writer). True by default.";
    }
}