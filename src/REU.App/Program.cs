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
using Contracts.Models;

namespace App;
internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var config = builder.Configuration;

        var appSettings = new AppSettings(config);

        builder.Services.AddSingleton(appSettings);
       
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options => options.FormatterName = "custom");

        builder.Logging.AddConsoleFormatter<CustomConsoleFormatter, ConsoleFormatterOptions>();
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

        using var app = builder.Build();
        
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

        var resolvedAppSettings = app.Services.GetRequiredService<AppSettings>();

        var rootCommand = new RootCommand("REU - Research and Execution Utility")
        {
            new RunPipelineCommand(resolvedAppSettings, loggerFactory),
            new RunSimulationCommand(resolvedAppSettings, loggerFactory)
        };

        return await rootCommand.Parse(args).InvokeAsync();
    }
}