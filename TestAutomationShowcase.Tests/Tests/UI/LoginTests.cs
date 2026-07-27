using Allure.NUnit;
using Allure.NUnit.Attributes;
using Microsoft.Playwright;
using TestAutomationShowcase.Configuration;
using TestAutomationShowcase.Core.PageObjects.SauceDemo;
using TestAutomationShowcase.Tests.Helpers;

namespace TestAutomationShowcase.Tests.Tests.UI;

/// <summary>
/// Contains UI tests for the login functionality of the SauceDemo application.
/// </summary>
/// <remarks>Tests include valid login, handling of locked-out users, and validation of empty credentials.</remarks>
[TestFixture]
[Parallelizable(ParallelScope.Self)]
[Category("UI")]
[AllureNUnit]
[AllureFeature("SauceDemo UI")]
[AllureSuite("LoginUIPlaywrightTests")]
public class LoginTests : PlaywrightBase
{
    private LoginPage _loginPage;

    [SetUp]
    public void SetUpPages() => _loginPage = new LoginPage(Page);

    [Test]
    public async Task ValidLogin_RedirectsToProductsPage()
    {
        await _loginPage.GoToAsync();
        await _loginPage.LoginAsync(ConfigReader.Settings.SauceDemoCredentialsStandard.Username, ConfigReader.Settings.SauceDemoCredentialsStandard.Password);

        Assert.That(Page.Url, Does.Contain("inventory.html"));
    }

    public static IEnumerable<TestCaseData> InvalidCredentials()
    {
        yield return new TestCaseData(ConfigReader.Settings.SauceDemoCredentialsLocked.Username, ConfigReader.Settings.SauceDemoCredentialsLocked.Password, "Epic sadface: Sorry, this user has been locked out.")
            .SetName("Locked out user");

        yield return new TestCaseData("", "", "Epic sadface: Username is required")
            .SetName("Empty credentials");

        yield return new TestCaseData(ConfigReader.Settings.SauceDemoCredentialsStandard.Username, "wrongpassword", "Epic sadface: Username and password do not match any user in this service")
            .SetName("Wrong password");
    }

    [Test]
    [TestCaseSource(nameof(InvalidCredentials))]
    public async Task InvalidCredentials_ShowsErrorMessage(string username, string password, string errorMessage)
    {
        await _loginPage.GoToAsync();
        await _loginPage.LoginAsync(username, password);

        await Assertions.Expect(_loginPage.Error).ToHaveTextAsync(errorMessage);
    }
}