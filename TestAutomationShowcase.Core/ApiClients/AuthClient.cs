using System.Net.Http.Json;
using TestAutomationShowcase.Configuration;

namespace TestAutomationShowcase.Core.ApiClients;

/// <summary>
/// Provides authentication functionality for obtaining tokens from a RESTful API.
/// </summary>
public class AuthClient
{
    private readonly HttpClient _httpClient;

    public AuthClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetTokenAsync()
    {
        var request = new
        {
            username = ConfigReader.Settings.RestfulBookerCredentials.Username,
            password = ConfigReader.Settings.RestfulBookerCredentials.Password
        };

        var response = await _httpClient.PostAsJsonAsync("/auth", request);

        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();

        return auth!.Token;
    }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
}
