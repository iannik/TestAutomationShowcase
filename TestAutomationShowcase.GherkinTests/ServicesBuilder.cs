using Reqnroll.BoDi;
using System.Net.Http.Headers;
using TestAutomationShowcase.Configuration;
using TestAutomationShowcase.Core;
using TestAutomationShowcase.Core.ApiClients;

namespace TestAutomationShowcase.GherkinTests
{
    /// <summary>
    /// Provides methods to register and configure application services for dependency injection.
    /// </summary>
    public sealed class ServicesBuilder
    {
        private readonly IObjectContainer _container;
        public ServicesBuilder(IObjectContainer container)
        {
            _container = container;
        }

        public void Build()
        {
            _container.RegisterTypeAs<ApiTestLogger, ApiTestLogger>();

            _container.RegisterInstanceAs(new AuthClient(CreateAuthHttpClient()));
            _container.RegisterTypeAs<TokenProvider, TokenProvider>();
            _container.RegisterTypeAs<AuthHandler, AuthHandler>();

            _container.RegisterTypeAs<LoggingHandler, LoggingHandler>();

            _container.RegisterTypeAs<HttpClientFactory, HttpClientFactory>();

            var factory = _container.Resolve<HttpClientFactory>();
            var client = factory.Create();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _container.RegisterInstanceAs(client);
            _container.RegisterInstanceAs(new BookingClient(client));
        }

        private static HttpClient CreateAuthHttpClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri(ConfigReader.Settings.ApiBaseUrl)
            };
        }
    }
}
