using System.CommandLine;

namespace App.Options;
public class OutputPathOption : Option<string?>
{
    public OutputPathOption() : base("--output", "-o")
    {
        Description = "Overrides the output path for the pipeline from appsettings.json.";
    }
}