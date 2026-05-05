using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.CommandLine;
using App.Commands;
using App.Logging;


internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options => options.FormatterName = "custom");

        builder.Logging.AddConsoleFormatter<CustomConsoleFormatter, ConsoleFormatterOptions>();
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

        using var app = builder.Build();
        
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        var rootCommand = new RootCommand("REU Data Processing Application")
        {
            new RunPipelineCommand()
        };

        return await rootCommand.Parse(args).InvokeAsync();
    }
}