using NUnit.Framework;
using ConfigManagement;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework.Legacy;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Metrics;
using Serilog;

namespace ConfigHubTests
{
    public class ConfigHubTests
    {   
        private NullLogger<ConfigHub> _logger;
        private ConfigHub _configManager;
        [SetUp]
        public void Setup()
        {
            _logger = NullLogger<ConfigHub>.Instance;
           
            _configManager = new ConfigHub(_logger);
        }

        [Test]
        public void ConfirmSingletonTest()
        {
            ClassicAssert.AreSame(_configManager, new ConfigHub(_logger));
        }
    }
}

    