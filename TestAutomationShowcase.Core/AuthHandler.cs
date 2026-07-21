namespace TestAutomationShowcase.Core;

/// <summary>
/// Delegating handler that adds an authentication token to HTTP request headers for specific HTTP methods.
/// </summary>
/// <remarks>Skips authentication if the request specifies the SkipAuthentication option.</remarks>
public class AuthHandler : DelegatingHandler
{
    private readonly TokenProvider _tokenProvider;

    public AuthHandler(TokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var skipAuth = request.Options.TryGetValue(HttpRequestOptions.SkipAuthentication, out var value) && value;

        if (!skipAuth && (request.Method == HttpMethod.Put || request.Method == HttpMethod.Delete || request.Method == HttpMethod.Patch))
        {
            var token = await _tokenProvider.GetTokenAsync();
            request.Headers.Add("Cookie", $"token={token}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
