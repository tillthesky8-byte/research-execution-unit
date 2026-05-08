using System.CommandLine;

namespace App.Options;
public class InitialCashOption : Option<decimal?>
{
    public InitialCashOption() : base("--initial-cash", "-ic")
    {
        Description = "Specifies the initial cash amount for the simulation.";
    }
}