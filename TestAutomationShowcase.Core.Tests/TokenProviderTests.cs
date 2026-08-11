using NSubstitute;
using TestAutomationShowcase.Core.ApiClients;

namespace TestAutomationShowcase.Core.Tests;

/// <summary>
/// Contains unit tests for the TokenProvider class.
/// </summary>
public class TokenProviderTests
{
    /// <summary>
    /// Verifies that GetTokenAsync returns the expected token from the authentication client.
    /// </summary>
    [Test]
    public async Task GetTokenAsync_ReturnsToken_FromAuthClient()
    {
        var authClient = Substitute.For<IAuthClient>();
        authClient.GetTokenAsync().Returns("token-123");

        var tokenProvider = new TokenProvider(authClient);

        var token = await tokenProvider.GetTokenAsync();

        Assert.That(token, Is.EqualTo("token-123"));
    }

    /// <summary>
    /// Verifies that GetTokenAsync caches the token and does not invoke the authentication client more than once.
    /// </summary>
    [Test]
    public async Task GetTokenAsync_CachesToken_DoesNotCallAuthClientTwice()
    {
        var authClient = Substitute.For<IAuthClient>();
        authClient.GetTokenAsync().Returns("token-123");
        var tokenProvider = new TokenProvider(authClient);

        await tokenProvider.GetTokenAsync();
        await tokenProvider.GetTokenAsync();

        await authClient.Received(1).GetTokenAsync();
    }

    /// <summary>
    /// Verifies that multiple concurrent calls to GetTokenAsync result in only a single token retrieval from the authentication client.
    /// </summary>
    [Test]
    public async Task GetTokenAsync_ConcurrentCalls_OnlyFetchTokenOnce()
    {
        var callCount = 0;
        var authClient = Substitute.For<IAuthClient>();
        authClient.GetTokenAsync().Returns(async _ =>
        {
            callCount++;
            await Task.Delay(50);
            return "token-123";
        });

        var tokenProvider = new TokenProvider(authClient);

        var tasks = new List<Task>();
        for (var i = 0; i < 20; i++)
            tasks.Add(tokenProvider.GetTokenAsync());

        await Task.WhenAll(tasks);

        Assert.That(callCount, Is.EqualTo(1));
    }
}