using System.CommandLine;

namespace App.Options;
public class ConnectionStringOption : Option<string?>
{
    public ConnectionStringOption() : base("--connection-string", "-cs")
    {
        Description = "Overrides the connection string for the pipeline from appsettings.json.";
    }
}