using System;
using System.Threading;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ConfigManagement
{
  //Double check lock and lazy initialization for safe multithreading
  class ConfigHub
  {
    // a lock object that will be used to synchronize threads during first access to the Singleton
    private static readonly object _lock = new object();
    

    private static ConfigHub _instance;
    public static ConfigHub Instance
    {
      get
      {
        if (_instance == null)
        {
          lock (_lock)
          {
            if (_instance == null)
            {
              _instance = new ConfigHub();
            }
          }
        }
        return _instance;
      }
    }

    private ConfigHub(){} 

    public void LoadFromJson(object serialiazableObject, string filePath)
    {

    }

    public void LoadFromYaml(string filePath)
    {
      
    }

    public void SaveToJson<T>(T serialiazableObject, string filePath)
    {
      string fileName = $"{filePath}/{serialiazableObject.GetType().Name}.json";
      var options = new JsonSerializerOptions { WriteIndented = true };
      string jsonString = JsonSerializer.Serialize<T>(serialiazableObject, options);
      File.WriteAllText(fileName, jsonString);
    }

    public void SaveToYaml(string filePath)
    {

    }
    
  }
}

