using Microsoft.Playwright;
using Reqnroll;
using TestAutomationShowcase.Configuration;
using TestAutomationShowcase.Core.PageObjects.SauceDemo;
using TestAutomationShowcase.GherkinTests.Context;

[Binding]
public class LoginSteps
{
    private readonly WebContext _webContext;
    private LoginPage _loginPage;

    public LoginSteps(WebContext webContext)
    {
        _webContext = webContext;
        _loginPage = new LoginPage(_webContext.Page);
    }

    /// <summary>
    /// Navigates to the SauceDemo login page.
    /// </summary>
    [Given("the user is on the SauceDemo login page")]
    public async Task UserIsOnLoginPage()
    {
        await _loginPage.GoToAsync();
    }

    /// <summary>
    /// Logs in the user with the specified credentials type (standard, locked out, empty credential, wrong password).
    /// </summary>
    /// <param name="credentialsType">Credentials type</param>
    [Given("the user logs in as a {string} user")]
    [When("the user logs in as a {string} user")]
    public async Task UserLogsInAs(string credentialsType)
    {
        var (username, password) = credentialsType switch
        {
            "standard" => (ConfigReader.Settings.SauceDemoCredentialsStandard.Username, ConfigReader.Settings.SauceDemoCredentialsStandard.Password),
            "locked out" => (ConfigReader.Settings.SauceDemoCredentialsLocked.Username, ConfigReader.Settings.SauceDemoCredentialsLocked.Password),
            "empty credential" => (string.Empty,string.Empty),
            "wrong password" => (ConfigReader.Settings.SauceDemoCredentialsStandard.Username, "wrongpassword"),
            _ => throw new ArgumentException($"Unknown credentials type: '{credentialsType}'")
        };

        await _loginPage.LoginAsync(username, password);
    }

    /// <summary>
    /// Verifies that the user is redirected to the Products page.
    /// </summary>
    [Then("they should be redirected to the Products page")]
    public async Task RedirectedToProducts()
    {
        await Assertions.Expect(_webContext.Page).ToHaveURLAsync($"{ConfigReader.Settings.UiBaseUrl}/inventory.html");
    }

    /// <summary>
    /// Verifies that the login error message is displayed on the page.
    /// </summary>
    [Then("the login error message should be displayed")]
    public async Task ErrorDisplayed()
    {
        await Assertions.Expect(_loginPage.Error).ToBeVisibleAsync();
    }

    /// <summary>
    /// Verifies that the login error message is displayed with the specified text.
    /// </summary>
    /// <param name="errorMessage">Expected message text</param>
    [Then("the login error message should read {string}")]
    public async Task ErrorDisplayedWithText(string errorMessage)
    {
        await Assertions.Expect(_loginPage.Error).ToHaveTextAsync(errorMessage);
    }
}
