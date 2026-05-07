using System.CommandLine;
namespace App.Options;

public class EndDateOption : Option<DateTime>
{
    public EndDateOption() : base("--end-date", "-end")
    {
        Description = "Specifies the end date for the dataset (inclusive). Format: YYYY-MM-DD.";
    }
}