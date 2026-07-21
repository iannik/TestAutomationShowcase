using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using TestAutomationShowcase.Configuration;
using TestAutomationShowcase.Core;
using TestAutomationShowcase.Core.ApiClients;

namespace TestAutomationShowcase.Tests.Helpers
{
    /// <summary>
    /// Provides extension methods for registering core application services with the dependency injection container.
    /// </summary>
    public static class ServiceRegistration
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddSingleton<ApiTestLogger>();
            services.AddSingleton<TokenProvider>();

            services.AddTransient<LoggingHandler>();
            services.AddTransient<AuthHandler>();

            services.AddHttpClient<AuthClient>(client =>
            {
                client.BaseAddress = new Uri(ConfigReader.Settings.ApiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddHttpClient<BookingClient>(client =>
            {
                client.BaseAddress = new Uri(ConfigReader.Settings.ApiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
                    .AddHttpMessageHandler<AuthHandler>()
                    .AddHttpMessageHandler<LoggingHandler>();

            return services;
        }
    }
}
