using NUnit.Framework;
using NUnit.Framework.Legacy; // For ClassicAssert
using ConfigManagement;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ConfigHubApp.Settings; 
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConfigHubTests
{
    public class ConfigHubTests
    {
        private ConfigHub _configManager;
        private string _tempDirectory = null!;

        [SetUp]
        public void Setup()
        {
            // 1) Create minimal IOptions<AppSettings> if needed
            var appSettings = new AppSettings();
            var options = Options.Create(appSettings);

            // 2) Provide a logger (NullLogger for no console spam)
            ILogger<ConfigHub> logger = NullLogger<ConfigHub>.Instance;

            // 3) Construct ConfigHub
            _configManager = new ConfigHub(options, logger);

            // 4) Create a temp directory for file I/O
            _tempDirectory = Path.Combine(Path.GetTempPath(), "ConfigHubTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDirectory);
        }

        [TearDown]
        public void Teardown()
        {
            // Cleanup the temp test directory
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
        }

        [Test]
        public void ConfirmSingletonTest()
        {
            Assert.That(_configManager, Is.Not.Null);
            ClassicAssert.AreSame(_configManager, _configManager);
        }

        [Test]
        public void LoadFromJson_ValidFile_Succeeds()
        {
            // Arrange
            var filePath = Path.Combine(_tempDirectory, "test.json");
            var content = @"{ ""TestValue"": 123 }";
            File.WriteAllText(filePath, content);

            // Act
            var result = _configManager.LoadFromJson<TestData>(filePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TestValue, Is.EqualTo(123));
        }

        [Test]
        public void LoadFromJson_FileDoesNotExist_Throws()
        {
            // Arrange
            var nonExistentFile = Path.Combine(_tempDirectory, "no_such_file.json");

            // Act & Assert
            Assert.Throws<FileNotFoundException>(
                () => _configManager.LoadFromJson<TestData>(nonExistentFile)
            );
        }

        [Test]
        public void SaveToJson_ValidObject_CreatesFile()
        {
            // Arrange
            var outDir = Path.Combine(_tempDirectory, "output");
            Directory.CreateDirectory(outDir);

            var data = new TestData { TestValue = 42 };

            // Act
            // Pass the folder path, so ConfigHub will create a file named "TestData.json" inside that folder
            _configManager.SaveToJson(data, outDir);

            // Assert
            var finalPath = Path.Combine(outDir, "TestData.json");
            Assert.That(File.Exists(finalPath), Is.True,
                $"Expected file TestData.json to be created in '{outDir}'");

            // Optionally load back and verify
            var reloaded = _configManager.LoadFromJson<TestData>(finalPath);
            Assert.That(reloaded.TestValue, Is.EqualTo(42));
        }

        [Test]
        public async Task LoadFromJsonAsync_ValidFile_Succeeds()
        {
            // Arrange
            var filePath = Path.Combine(_tempDirectory, "asyncTest.json");
            var content = @"{ ""TestValue"": 999 }";
            File.WriteAllText(filePath, content);

            // Act
            var result = await _configManager.LoadFromJsonAsync<TestData>(filePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TestValue, Is.EqualTo(999));
        }

        [Test]
        public async Task SaveToJsonAsync_ValidObject_CreatesFile()
        {
            // Arrange
            var outDir = Path.Combine(_tempDirectory, "asyncOutput");
            Directory.CreateDirectory(outDir);

            var data = new TestData { TestValue = 777 };

            // Act
            await _configManager.SaveToJsonAsync(data, outDir);

            // Assert
            var finalPath = Path.Combine(outDir, "TestData.json");
            Assert.That(File.Exists(finalPath), Is.True, $"Expected {finalPath} to be created.");

            var reloaded = _configManager.LoadFromJson<TestData>(finalPath);
            Assert.That(reloaded.TestValue, Is.EqualTo(777));
        }

        [Test]
        public void LoadFromYaml_ValidFile_Succeeds()
        {
            // Arrange
            var filePath = Path.Combine(_tempDirectory, "test.yaml");
            // Use "testValue" (camelCase) to match YamlDotNet's CamelCaseNamingConvention
            var yaml = "testValue: 321";
            File.WriteAllText(filePath, yaml);

            // Act
            var result = _configManager.LoadFromYaml<TestData>(filePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TestValue, Is.EqualTo(321));
        }

        [Test]
        public void SaveToYaml_ValidObject_CreatesFile()
        {
            // Arrange
            var outDir = Path.Combine(_tempDirectory, "yamlOutput");
            Directory.CreateDirectory(outDir);

            var data = new TestData { TestValue = 2023 };

            // Act
            _configManager.SaveToYaml(data, outDir);

            // Assert
            var finalPath = Path.Combine(outDir, "TestData.yml");
            Assert.That(File.Exists(finalPath), Is.True, $"Expected {finalPath} to be created.");

            // Optionally re-deserialize with your same method:
            var reloaded = _configManager.LoadFromYaml<TestData>(finalPath);
            Assert.That(reloaded.TestValue, Is.EqualTo(2023));
        }

        // Helper class for testing
        private class TestData
        {
            public int TestValue { get; set; }
        }
    }
}
