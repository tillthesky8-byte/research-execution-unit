using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using App.Logging;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options => options.FormatterName = "custom");

        builder.Logging.AddConsoleFormatter<CustomConsoleFormatter, ConsoleFormatterOptions>();
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

        using var app = builder.Build();
        
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("This is INFORMATION.");
        logger.LogWarning("This is WARNING.");
        logger.LogError("This is ERROR.");
        logger.LogDebug("This is DEBUG.");
        logger.LogTrace("This is TRACE.");

        return;
    }
}