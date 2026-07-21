using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TestAutomationShowcase.Core.Models;

namespace TestAutomationShowcase.Core.ApiClients;

/// <summary>
/// Provides a base class for API clients that manage HTTP requests and responses.
/// </summary>
/// <remarks>Intended to be inherited by specific API client implementations to standardize HTTP communication and response handling.</remarks>
public abstract class BaseApiClient
{
    protected readonly HttpClient Http;
    private static readonly JsonSerializerOptions SerializerOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    protected BaseApiClient(HttpClient http)
    {
        Http = http;
    }

    protected async Task<ApiResponse<T>> SendAsync<T>(HttpMethod method, string uri, object? body = null, bool skipAuth = false)
    {
        var request = new HttpRequestMessage(method, uri);

        if (skipAuth)
        {
            request.Options.Set(HttpRequestOptions.SkipAuthentication, true);
        }

        if (body != null)
        {
            request.Content = Serialize(body);
        }

        var response = await Http.SendAsync(request);

        var content = await response.Content.ReadAsStringAsync();

        T? value = default;

        if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(content))
        {
            value = await DeserializeAsync<T>(response);
        }

        return new ApiResponse<T>(response.StatusCode, value, content);
    }

    private static StringContent Serialize(object body)
    {
        return new StringContent(JsonSerializer.Serialize(body, SerializerOptions), Encoding.UTF8, "application/json");
    }

    private static async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException($"Failed to deserialize response to {typeof(T).Name}");
    }
}
