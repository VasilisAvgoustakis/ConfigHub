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

                services.AddEndpointsApiExplorer();
            })
            .Build();


        
        //var hub = host.Services.GetRequiredService<ConfigHub>();

        // 4) Now we use the built host's Services to build a minimal API pipeline
        var app = WebApplication.CreateBuilder(args)
                                .Build();

        // 5) Set up minimal API security (very basic)
        app.Use(async (context, next) =>
        {
            // This could also come from appsettings if you want
            var expectedApiKey = "your-dev-api-key";

            var apiKey = context.Request.Headers["x-api-key"].FirstOrDefault();
            if (apiKey != expectedApiKey)
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Invalid or missing API key.");
                return;
            }
            await next();
        });

        // 6) Map endpoints
        // Note: The DI container from the host is accessible via "host.Services"
        var configHub = host.Services.GetRequiredService<ConfigHub>();

        app.MapGet("/configs", () =>
        {
            // Suppose you store everything in _configStore as dictionary of name -> object
            var configNames = configHub.GetAllConfigNames(); // e.g. some method to list keys
            return Results.Ok(configNames);
        });

        app.MapGet("/configs/{name}", (string name) =>
        {
            // Retrieve the config if it exists
            var config = configHub.GetConfig(name); // e.g. some method
            if (config is null)
            {
                return Results.NotFound($"No config found for '{name}'.");
            }
            return Results.Ok(config);
        });

        app.MapPut("/configs/{name}", async (string name, HttpRequest request) =>
        {
            // For simplicity, let's assume body is a JSON object we want to store
            // in a file named after 'name' plus .json
            using var reader = new StreamReader(request.Body);
            var bodyString = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(bodyString))
            {
                return Results.BadRequest("Empty request body.");
            }

            // Save to JSON (the method can do whatever path logic it wants)
            try
            {
                configHub.SaveToJson(bodyString, name); 
                // e.g. or configHub.SaveToJson<T>(parsedObject, "some/path")
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Failed to store config. Error: {ex.Message}");
            }

            return Results.Ok($"Config for {name} saved successfully.");
        });

        // 7) Start the minimal API
        await app.RunAsync();

        
    }
}
