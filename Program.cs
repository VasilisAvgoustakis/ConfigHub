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
    
    
  public static void Main()
  { 
    ConfigHub configManager = ConfigHub.Instance;

    var WeatherForecast = new WeatherForecast()
    {
      Date = DateTime.Parse("2019-08-01"),
      TemperatureCelsius = 25,
      Summary = "Hot"
    };

    configManager.SaveToJson(WeatherForecast, "./temp");
    
  }

}

