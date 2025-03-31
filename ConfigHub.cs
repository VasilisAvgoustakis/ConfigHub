using System;
using System.Threading;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO;
using Serilog;

namespace ConfigManagement
{
  //Double check lock and lazy initialization for safe multithreading
  class ConfigHub
  {
    // a lock object that will be used to synchronize threads during first access to the Singleton
    private static readonly object _lock = new object();
    

    private static ConfigHub? _instance;

    private static JsonSerializerOptions? _options;
    public static ConfigHub Instance
    {
      get
      {
        if (_instance == null)
        {
          lock (_lock)
          {
            _instance ??= new ConfigHub();
          }
        }
        return _instance;
      }
    }

    private ConfigHub()
    {
      // Setup Logger
      Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/_log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
      Log.Information("ConfigHub Singleton created!");

      // initialize Serializer options
      _options = new JsonSerializerOptions{WriteIndented = true};
    } 

    // Deserializes from json text file
    public static T LoadFromJson<T>(string filePath)
    {
      try
      {
        string? jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(jsonString)!; 
      }
      catch(Exception ex)
      {
        // log the exception and rethrow, we leave it to the caller to decide how to handle the exception
        Log.Error("Error during Deserialization:", ex);
        throw;
      }
    }

    public static async Task<T?> LoadFromJsonAsync<T>(string filePath)
    {
      try
      {
        using FileStream openStream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<T>(openStream);
      }
      catch (Exception ex)
      {
        Log.Error("Error during async Deserialization: ", ex);
        throw;
      }
    }

    public void LoadFromYaml(string filePath)
    {
      
    }

    public static void SaveToJson<T>(T serialiazableObject, string filePath)
    {
      try
      {
        string fileName = $"{filePath}/{serialiazableObject?.GetType().Name}.json";
        string jsonString = JsonSerializer.Serialize<T>(serialiazableObject, _options);
        File.WriteAllText(fileName, jsonString); 
        Log.Information("Serialized to JSON file: ", fileName);
      }
      catch (Exception ex)
      {
        Log.Error("Error while Serializing to Json Text file: ", ex);
        throw;
      }
    }

    public static async Task SaveToJsonAsync<T>(T serialiazableObject, string filePath)
    {
      try
      {
        string fileName = $"{filePath}/{serialiazableObject?.GetType().Name}.json";
        await using FileStream createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, serialiazableObject, _options);
        Log.Information("Serialized Async to File: ", fileName);  
      }
      catch (Exception ex)
      {
        Log.Error("Error during async Serialization: ", ex);
      }
    }

    public void SaveToYaml(string filePath)
    {

    }
    
  }
}

