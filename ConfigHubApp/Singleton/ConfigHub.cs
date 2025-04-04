using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Microsoft.Extensions.Logging;
using Serilog;
using ConfigHubApp.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


namespace ConfigManagement
{
  //Double check lock and lazy initialization for safe multithreading
  public class ConfigHub
  {
    private readonly IOptions<AppSettings> _settings;
    private ILogger<ConfigHub> _logger;
    private static JsonSerializerOptions? _options;
    private readonly ISerializer _yamlSerializer;
    private readonly IDeserializer _yamlDeserializer;
    private Dictionary<string, object> _configStore;
    public Dictionary<string, object> ConfigStore {
      get
      {
        return _configStore;
      }
      }

    //private Lazy<ConfigHub> _instanceLazy; 
    //public ConfigHub Instance => _instanceLazy.Value; //This approach automatically handles thread-safe, lazy instantiation, removing the need for manual locking.

    public ConfigHub(IOptions<AppSettings> settings, ILogger<ConfigHub> logger)
    {
      _settings = settings;
      _logger = logger;
      //_instanceLazy = new (() => new ConfigHub(_settings, _logger));
      
      _logger.LogInformation("ConfigHub initialized with environment: {Env}", _settings.Value.Environment);

      // initialize Serializer options
      _options = new JsonSerializerOptions{WriteIndented = true};

      // initialize _configStore
      _configStore = new();

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
        // read all text from file
        string? jsonString = File.ReadAllText(filePath);
        // deserialize to T object
        T jsonObject = JsonSerializer.Deserialize<T>(jsonString)!;  
        // keep json object in memory for quick access
        _configStore.Add(jsonObject.GetType().Name, jsonObject);
        // finally return the object
        return jsonObject;
      }
      catch(Exception ex)
      {
        // log the exception and rethrow, we leave it to the caller to decide how to handle the exception
        _logger.LogError(ex, "Error during Deserialization");
        throw;
      }
    }

    public async Task<T> LoadFromJsonAsync<T>(string filePath)
    {
      try
      {
        // Create a stream of the file's contents
        using FileStream openStream = File.OpenRead(filePath);
        // asynchronously create a jsonObject
        T? jsonObject = await JsonSerializer.DeserializeAsync<T>(openStream); 
        // add it to memory for quick access 
        _configStore.Add(jsonObject.GetType().Name, jsonObject);
        // returne the jsonObject
        return jsonObject;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error during async Deserialization");
        throw;
      }
    }


    public T LoadFromYaml<T>(string filePath)
    {
      try
      {
        string? yamlString = File.ReadAllText(filePath);
        T yamlObject = _yamlDeserializer.Deserialize<T>(yamlString); 
        _configStore.Add(yamlObject.GetType().Name, yamlObject);
        return yamlObject;
      }
      catch(Exception ex)
      {
        _logger.LogError(ex, "Error during YAML Deserialization");
        throw;
      }
    }


    public void SaveToJson<T>(T serializableObject, string filePath)
    {
      try
      {
        string fileName = BuildFileNameFromPath(serializableObject, filePath, "json");
        string jsonString = JsonSerializer.Serialize<T>(serializableObject, _options);
        File.WriteAllText(fileName, jsonString); 
        _logger.LogInformation($"Serialized to JSON file:{fileName}");
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
        string fileName = BuildFileNameFromPath(serializableObject, filePath, "json");
        await using FileStream createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, serializableObject, _options);
        _logger.LogInformation($"Serialized Async to JSON File:{fileName} ");  
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
        string fileName = BuildFileNameFromPath(serializableObject, filePath, "yml");
        string yamlString = _yamlSerializer.Serialize(serializableObject);
        File.WriteAllText(fileName, yamlString); 
        _logger.LogInformation($"Serialized to YAML file: {fileName} ");
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error during Serilization to YAML File.");
        throw;
      }
    }   


    // Helper function to build file from path, object name and format type
    private static string BuildFileNameFromPath<T>(T serializableObject, string filePath, string fileType)
    {
      string fileName = $"{filePath}/{serializableObject?.GetType().Name}.{fileType}";
      return fileName;
    }
  }
}

