using Microsoft.Extensions.Configuration;
using TestAutomationShowcase.Configuration.Models;

namespace TestAutomationShowcase.Configuration
{
    public static class ConfigReader
    {
        private static readonly AppSettings _settings;

        static ConfigReader()
        {
            var configDir = Path.GetDirectoryName(typeof(ConfigReader).Assembly.Location)!;

            var config = new ConfigurationBuilder().SetBasePath(configDir)
                                                   .AddJsonFile("appsettings.json", optional: false)
                                                   .AddJsonFile("appsettings.local.json",optional: true)
                                                   .AddEnvironmentVariables()
                                                   .Build();

            _settings = config.Get<AppSettings>() ?? throw new InvalidOperationException("Failed to load configuration.");
        }

        public static AppSettings Settings => _settings;
    }
}