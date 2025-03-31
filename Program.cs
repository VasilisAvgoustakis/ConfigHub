using System;
using ConfigManagement;
using System.Threading.Tasks;


public class Program
{

  public class WeatherForecast
  {
      public DateTimeOffset Date { get; set; }
      public int TemperatureCelsius { get; set; }
      public string? Summary { get; set; }
  }
    
    
  public static async Task Main()
  { 
    // get the Singleton instance of the ConfigHub class
    ConfigHub configManager = ConfigHub.Instance;
    
    // create a WeatherForecast instance
    var weatherForecast = new WeatherForecast()
    {
      Date = DateTime.Parse("2019-08-01"),
      TemperatureCelsius = 25,
      Summary = "Hot"
    };

    // serialize the WeatherForecast Object to a json text file
    // Console.WriteLine("Serializing to Temp: ");
    // configManager.SaveToJson(weatherForecast, "./temp");

    // deserialize from json
    // Console.WriteLine("Deserializing: ");
    // weatherForecast = configManager.LoadFromJson<WeatherForecast>("./temp/WeatherForecast.json");
    // Console.WriteLine($"Date: {weatherForecast?.Date}");
    // Console.WriteLine($"TemperatureCelsius: {weatherForecast?.TemperatureCelsius}");
    // Console.WriteLine($"Summary: {weatherForecast?.Summary}");

    // Console.WriteLine("Serializing Async: ");
    // await configManager.SaveToJsonAsync(weatherForecast, "./temp");

    // Console.WriteLine("Deserializing Async: ");
    // weatherForecast = await configManager.LoadFromJsonAsync<WeatherForecast>("./temp/WeatherForecast.json");
    // Console.WriteLine($"Date: {weatherForecast?.Date}");
    // Console.WriteLine($"TemperatureCelsius: {weatherForecast?.TemperatureCelsius}");
    // Console.WriteLine($"Summary: {weatherForecast?.Summary}");

    Console.WriteLine("Serilizing to yaml: ");
    configManager.SaveToYaml(weatherForecast, "./temp");

  }

}

