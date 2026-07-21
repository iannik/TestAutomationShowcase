using TestAutomationShowcase.Core.ApiClients;

namespace TestAutomationShowcase.Core;

/// <summary>
/// Provides thread-safe retrieval and caching of authentication tokens.
/// </summary>
/// <remarks>Ensures only one token is fetched and reused across concurrent requests.</remarks>
public class TokenProvider
{
    private readonly AuthClient _authClient;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private string? _token;

    public TokenProvider(AuthClient authClient)
    {
        _authClient = authClient;
    }

    public async Task<string> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(_token))
            return _token;

        await _lock.WaitAsync();

        try
        {
            if (string.IsNullOrEmpty(_token))
            {
                _token = await _authClient.GetTokenAsync();
            }

            return _token;
        }
        finally
        {
            _lock.Release();
        }
    }
}
