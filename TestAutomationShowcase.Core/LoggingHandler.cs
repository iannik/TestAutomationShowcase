namespace TestAutomationShowcase.Core;

/// <summary>
/// Handles HTTP request and response logging for API testing scenarios.
/// </summary>
/// <remarks>Uses the provided ApiTestLogger to log details of outgoing requests and incoming responses.</remarks>
public class LoggingHandler : DelegatingHandler
{
    private readonly ApiTestLogger _logger;

    public LoggingHandler(ApiTestLogger logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await LogRequestAsync(request);

        var response = await base.SendAsync(request, cancellationToken);

        await LogResponseAsync(response);

        return response;
    }

    private async Task LogRequestAsync(HttpRequestMessage request)
    {
        var body = request.Content is not null ? await request.Content.ReadAsStringAsync() : null;

        _logger.LogRequest(request.Method, request.RequestUri?.ToString() ?? string.Empty, body);
    }

    private async Task LogResponseAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        _logger.LogResponse(response.StatusCode, body);
    }
}
