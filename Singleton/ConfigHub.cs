using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Serilog;
using System.Threading.Tasks;

namespace ConfigManagement
{
  //Double check lock and lazy initialization for safe multithreading
  class ConfigHub
  {
    // a lock object that will be used to synchronize threads during first access to the Singleton
    private static readonly object _lock = new object();
    private static ConfigHub? _instance;
    private static JsonSerializerOptions? _options;
    private readonly ISerializer _yamlSerializer;
    private readonly IDeserializer _yamlDeserializer;

    private static readonly Lazy<ConfigHub> _instanceLazy = new (() => new ConfigHub());
    public static ConfigHub Instance => _instanceLazy.Value; //This approach automatically handles thread-safe, lazy instantiation, removing the need for manual locking.

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

      // initialize the yaml Serializer
      _yamlSerializer = new SerializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .WithIndentedSequences()
      .Build();

      // initialize the yaml deserializer
      _yamlDeserializer = new DeserializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .Build();
    } 

    // Deserializes from json text file
    public T LoadFromJson<T>(string filePath)
    {
      try
      {
        string? jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(jsonString)!; 
      }
      catch(Exception ex)
      {
        // log the exception and rethrow, we leave it to the caller to decide how to handle the exception
        Log.Error(ex, "Error during Deserialization");
        throw;
      }
    }

    public async Task<T?> LoadFromJsonAsync<T>(string filePath)
    {
      try
      {
        using FileStream openStream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<T>(openStream);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error during async Deserialization");
        throw;
      }
    }

    public T LoadFromYaml<T>(string filePath)
    {
      try
      {
        string? yamlString = File.ReadAllText(filePath);
        return _yamlDeserializer.Deserialize<T>(yamlString); 
      }
      catch(Exception ex)
      {
        Log.Error(ex, "Error during YAML Deserialization");
        throw;
      }
    }


    public void SaveToJson<T>(T serializableObject, string filePath)
    {
      try
      {
        string fileName = $"{filePath}/{serializableObject?.GetType().Name}.json";
        string jsonString = JsonSerializer.Serialize<T>(serializableObject, _options);
        File.WriteAllText(fileName, jsonString); 
        Log.Information("Serialized to JSON file: ", fileName);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error while Serializing to Json Text file");
        throw;
      }
    }

    public async Task SaveToJsonAsync<T>(T serializableObject, string filePath)
    {
      try
      {
        string fileName = $"{filePath}/{serializableObject?.GetType().Name}.json";
        await using FileStream createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, serializableObject, _options);
        Log.Information("Serialized Async to JSON File: ", fileName);  
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error during async Serialization");
      }
    }

    public void SaveToYaml<T>(T serializableObject, string filePath)
    {
      try
      {
        string fileName = $"{filePath}/{serializableObject?.GetType().Name}.yml";
        string yamlString = _yamlSerializer.Serialize(serializableObject);
        File.WriteAllText(fileName, yamlString); 
        Log.Information("Serialized to YAML file: ", fileName);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error during Serilization to YAML File.");
        throw;
      }
    } 
  }
}

