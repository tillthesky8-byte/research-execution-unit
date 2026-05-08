using System.CommandLine;

namespace App.Options;

public sealed class YamlConfigNameOption : Option<string?>
{
    public YamlConfigNameOption() : base("--yaml-config", "-yc")
    {
        Description = "The name of the YAML configuration file (without extension) located in the 'configs' directory. This option allows you to specify a predefined configuration for running the simulation. The application will look for a file named '<name>.yaml' in the 'configs' directory and use its contents to configure the pipeline and simulation parameters.";
        Arity = ArgumentArity.ZeroOrOne;
    }
}