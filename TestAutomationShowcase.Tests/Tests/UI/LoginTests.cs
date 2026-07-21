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
[AllureSuite("Login UI Playwright Tests")]
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

    [Test]
    public async Task LockedOutUser_ShowsErrorMessage()
    {
        await _loginPage.GoToAsync();
        await _loginPage.LoginAsync(ConfigReader.Settings.SauceDemoCredentialsLocked.Username, ConfigReader.Settings.SauceDemoCredentialsLocked.Password);

        await Assertions.Expect(_loginPage.Error).ToHaveTextAsync("Epic sadface: Sorry, this user has been locked out.");
    }

    [Test]
    public async Task EmptyCredentials_ShowsErrorMessage()
    {
        await _loginPage.GoToAsync();
        await _loginPage.LoginAsync(string.Empty, string.Empty);

        await Assertions.Expect(_loginPage.Error).ToHaveTextAsync("Epic sadface: Username is required");
    }
}