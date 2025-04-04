

namespace ConfigHubApp.Settings;


public class AppSettings
{
    public AppInfo App { get; set; } = new();
    public string Environment { get; set; } = "Development";
    public FeatureToggles Features { get; set; } = new();
    public PathSettings Paths { get; set; } = new();
    public LoggingSettings Logging { get; set; } = new();
    public CloudSettings Cloud { get; set; } = new();
    public RestApiSettings RestApi { get; set; } = new();
    public DashboardSettings Dashboard { get; set; } = new();
}

public class AppInfo
{
    public string Name { get; set; } = "ConfigHub";
    public string Version { get; set; } = "1.0.0";
}

public class FeatureToggles
{
    public bool EnableCloudStorage { get; set; }
    public bool EnableAuditLogging { get; set; }
    public bool EnableWebDashboard { get; set; }
    public bool EnableRestApi { get; set; }
}

public class PathSettings
{
    public string ConfigDirectory { get; set; } = "configs/";
    public string LogDirectory { get; set; } = "logs/";
    public string DefaultJsonFileName { get; set; } = "default-config.json";
    public string DefaultYamlFileName { get; set; } = "default-config.yaml";
}

public class LoggingSettings
{
    public string MinimumLevel { get; set; } = "Information";
    public string LogFilePath { get; set; } = "logs/_log.txt";
}

public class CloudSettings
{
    public string Provider { get; set; } = "Azure";
    public string ConnectionString { get; set; } = string.Empty;
}

public class RestApiSettings
{
    public bool Enabled { get; set; }
    public int Port { get; set; }
    public string ApiKey { get; set; } = string.Empty;
}

public class DashboardSettings
{
    public bool Enabled { get; set; }
    public string Host { get; set; } = "localhost";
    public int Port { get; set; }
}
