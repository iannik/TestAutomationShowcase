namespace TestAutomationShowcase.Core.ApiClients;

public interface IAuthClient
{
    Task<string> GetTokenAsync();
}
