using TestAutomationShowcase.Configuration;

namespace TestAutomationShowcase.Core;

/// <summary>
/// Provides a factory for creating HttpClient instances configured with logging and authentication handlers.
/// </summary>
public class HttpClientFactory
{
    private readonly LoggingHandler _loggingHandler;
    private readonly AuthHandler _authHandler;

    public HttpClientFactory(LoggingHandler loggingHandler, AuthHandler authHandler)
    {
        _loggingHandler = loggingHandler;
        _authHandler = authHandler;
    }

    public HttpClient Create()
    {
        _authHandler.InnerHandler = _loggingHandler;
        _loggingHandler.InnerHandler = new HttpClientHandler();

        return new HttpClient(_authHandler)
        {
            BaseAddress = new Uri(ConfigReader.Settings.ApiBaseUrl)
        };
    }
}
