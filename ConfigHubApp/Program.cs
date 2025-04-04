using ConfigHubApp.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ConfigManagement;
using Serilog;

public class Program
{
    public static async Task Main()
    {
        // Create Host
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false)
                      .AddEnvironmentVariables();
            })
            .UseSerilog((context, services, configuration) =>
            {
                var logPath = context.Configuration["Logging:LogFilePath"] ?? "logs/_log.txt";
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .WriteTo.Console()
                    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day);
            })
            .ConfigureServices((context, services) =>
            {
                // Bind the full AppSettings tree
                services.Configure<AppSettings>(context.Configuration);

                // Register your services
                services.AddSingleton<ConfigManagement.ConfigHub>(); // or however you want to register it
            })
            .Build();


        
        var hub = host.Services.GetRequiredService<ConfigHub>();

        // Run the host
        await host.RunAsync();

        
    }
}
