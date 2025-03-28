using System;
using System.Threading;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ConfigHub
{
  //Double check lock and lazy initialization for safe multithreading
  class ConfigHub
  {
    private ConfigHub(){} 

    private static ConfigHub _instance;

    // a lock object that will be used to synchronize threads during first access to the Singleton
    private static readonly object _lock = new object();

    public static getInstance()
    {
      if (_instance == null)
      {
        lock (_lock)
        {
          if (_nstance == null)
          {
            _instance = new ConfigHub();
          }
        }
      }
      return _instance;
    }


  }
}

