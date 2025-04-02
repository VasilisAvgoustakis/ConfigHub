using System;
using ConfigManagement;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;


public class Program
{

  public class WeatherForecast
  {
      public DateTime Date { get; set; }
      public int TemperatureCelsius { get; set; }
      public string? Summary { get; set; }
  }
    
    
  public static async Task Main()
  { 
    // Setup Logger
    Log.Logger = new LoggerConfiguration()
          .WriteTo.Console()
          .WriteTo.File("logs/_log.txt", rollingInterval: RollingInterval.Day)
          .MinimumLevel.Information()
          .Enrich.WithProperty("ApplicationContext", "ConfigHub")
          .CreateLogger();
    
    try
    {
      Log.Information("Staring ConfigHub Application...");

      // encapsulates an app's resources and lifetime functionality
      IHost host = Host.CreateDefaultBuilder()
                .UseSerilog() // 👈 Plug Serilog into ILogger
                .ConfigureServices((context, services) =>
                {   
                    services.AddSingleton<ConfigHub>();
                })
                .Build();
      
      host.Run();

      
      // get the Singleton instance of the ConfigHub class from Host
      ConfigHub? configManager = host.Services.GetRequiredService<ConfigHub>();

      // create a WeatherForecast instance
    var weatherForecast = new WeatherForecast()
    {
      Date = DateTime.Parse("2019-08-01"),
      TemperatureCelsius = 25,
      Summary = "Hot"
    };


      // //serialize the WeatherForecast Object to a json text file
      // Console.WriteLine("Serializing to Temp: ");
      // configManager.SaveToJson(weatherForecast, "./temp");

      // // deserialize from json
      // Console.WriteLine("Deserializing: ");
      // weatherForecast = configManager.LoadFromJson<WeatherForecast>("./temp/WeatherForecast.json");
      // Console.WriteLine($"Date: {weatherForecast?.Date}");
      // Console.WriteLine($"TemperatureCelsius: {weatherForecast?.TemperatureCelsius}");
      // Console.WriteLine($"Summary: {weatherForecast?.Summary}");

      Console.WriteLine("Serializing Async: ");
      await configManager.SaveToJsonAsync(weatherForecast, "./temp");

      Console.WriteLine("Deserializing Async: ");
      weatherForecast = await configManager.LoadFromJsonAsync<WeatherForecast>("./temp/WeatherForecast.json");
      Console.WriteLine($"Date: {weatherForecast?.Date}");
      Console.WriteLine($"TemperatureCelsius: {weatherForecast?.TemperatureCelsius}");
      Console.WriteLine($"Summary: {weatherForecast?.Summary}");

      // Console.WriteLine("Serilizing to yaml: ");
      // configManager.SaveToYaml(weatherForecast, "./temp");


      //   Console.WriteLine("Deserialiazing from yaml: ");
      //   WeatherForecast weatherForecast_ = configManager.LoadFromYaml<WeatherForecast>("./temp/WeatherForecast.yml");
      //   Console.WriteLine($"Date: {weatherForecast_?.Date}");
      //   Console.WriteLine($"TemperatureCelsius: {weatherForecast_?.TemperatureCelsius}");
      //   Console.WriteLine($"Summary: {weatherForecast_?.Summary}");
    }
    catch (Exception ex)
    {
      Log.Fatal(ex, "Application start-up failed!");
    }
    finally
    {
      Log.CloseAndFlush();
    }
  }

}

