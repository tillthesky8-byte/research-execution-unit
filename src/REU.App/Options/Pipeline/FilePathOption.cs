using System.CommandLine;

namespace App.Options;
public class FilePathOption : Option<string?>
{
    public FilePathOption() : base("--file", "-fp")
    {
        Description = "Overrides the data source file path for the pipeline from appsettings.json.";
    }
}