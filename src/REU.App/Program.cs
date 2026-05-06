using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.CommandLine;
using App.Commands;
using App.Logging;
using Contracts.Configs;
using Contracts.Enums;


internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var config = builder.Configuration;

        var pipelineConfig = new RunPipelineConfig
        {
            ConnectionString = config["ConnectionStrings:MarketData"]
                ?? throw new InvalidOperationException("Missing required configuration value 'ConnectionStrings:Default'."),
            FilePath = config["Paths:Source"]
                ?? throw new InvalidOperationException("Missing required configuration value 'Paths:Source'."),
            OutputPath = config["Paths:OutputPath"]
                ?? config["Paths:OutputRoot"]
                ?? throw new InvalidOperationException("Missing required configuration value 'Paths:OutputPath' or 'Paths:OutputRoot'."),
            LoaderType = Enum.TryParse(config["Pipeline:Loader"], ignoreCase: true, out LoaderType loaderType)
                ? loaderType
                : LoaderType.Sqlite,
            FuserType = Enum.TryParse(config["Pipeline:Fuser"], ignoreCase: true, out FuserType fuserType)
                ? fuserType
                : FuserType.LastObservationCarriedForward,
            WriterType = Enum.TryParse(config["Pipeline:Writer"], ignoreCase: true, out WriterType writerType)
                ? writerType
                : WriterType.CsvFile
        };

        builder.Services.AddSingleton(pipelineConfig);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options => options.FormatterName = "custom");

        builder.Logging.AddConsoleFormatter<CustomConsoleFormatter, ConsoleFormatterOptions>();
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

        using var app = builder.Build();
        
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

        var resolvedPipelineConfig = app.Services.GetRequiredService<RunPipelineConfig>();
        logger.LogInformation("Application started with configuration:");
        logger.LogInformation("ConnectionString: {valueColor}{ConnectionString}{reset}", ConsoleColors.ValueColor, resolvedPipelineConfig.ConnectionString, ConsoleColors.Reset);
        logger.LogInformation("FilePath:         {valueColor}{FilePath}{reset}", ConsoleColors.ValueColor, resolvedPipelineConfig.FilePath, ConsoleColors.Reset);
        logger.LogInformation("OutputPath:       {valueColor}{OutputPath}{reset}", ConsoleColors.ValueColor, resolvedPipelineConfig.OutputPath, ConsoleColors.Reset);
        logger.LogInformation("LoaderType:       {valueColor}{LoaderType}{reset}", ConsoleColors.ValueColor, resolvedPipelineConfig.LoaderType, ConsoleColors.Reset);
        logger.LogInformation("FuserType:        {valueColor}{FuserType}{reset}", ConsoleColors.ValueColor, resolvedPipelineConfig.FuserType, ConsoleColors.Reset);
        logger.LogInformation("WriterType:       {valueColor}{WriterType}{reset}", ConsoleColors.ValueColor, resolvedPipelineConfig.WriterType, ConsoleColors.Reset);


        var rootCommand = new RootCommand("REU Data Processing Application")
        {
            new RunPipelineCommand(resolvedPipelineConfig, loggerFactory)
        };

        return await rootCommand.Parse(args).InvokeAsync();
    }
}