using Microsoft.Extensions.DependencyInjection;
using TestAutomationShowcase.Core;

namespace TestAutomationShowcase.Tests.Helpers
{
    /// <summary>
    /// Provides access to core services and test dependencies through a static dependency injection container.
    /// </summary>
    /// <remarks>Not intended to be instantiated. Use Resolve<T> to obtain registered services for testing scenarios.</remarks>
    public static class TestServices
    {
        private static readonly IServiceProvider _provider;

        static TestServices()
        {
            var services = new ServiceCollection();
            services.AddCoreServices();
            services.AddScoped<ApiTestLogger>();
            _provider = services.BuildServiceProvider();
        }

        public static T Resolve<T>() where T : notnull => _provider.GetRequiredService<T>();
    }
}
