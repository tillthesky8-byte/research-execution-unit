using System.CommandLine;

namespace App.Options;

public class StartDateOption : Option<DateTime>
{
    public StartDateOption() : base("--start-date", "-start")
    {
        Description = "Specifies the start date for the dataset (inclusive). Format: YYYY-MM-DD.";
    }
}