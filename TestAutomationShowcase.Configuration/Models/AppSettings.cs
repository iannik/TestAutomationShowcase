namespace TestAutomationShowcase.Configuration.Models
{
    public class AppSettings
    {
        public string ApiBaseUrl { get; init; }
        public string UiBaseUrl { get; init; }
        public BrowserSettings Browser { get; init; }
        public CredentialsSettings RestfulBookerCredentials { get; init; }
        public CredentialsSettings SauceDemoCredentialsStandard { get; init; }
        public CredentialsSettings SauceDemoCredentialsLocked { get; init; }
    }

    public class BrowserSettings
    {
        public bool Headless { get; init; }
        public int SlowMo { get; init; }
        public int DefaultTimeout { get; init; }
    }

    public class CredentialsSettings
    {
        public string Username { get; init; }
        public string Password { get; init; }
    }
}
