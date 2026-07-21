using Microsoft.Playwright;
using TestAutomationShowcase.Configuration;

namespace TestAutomationShowcase.Tests.Helpers;

/// <summary>
/// Provides a base class for Playwright-based tests, managing the lifecycle of the Playwright instance, browser, and page.
/// </summary>
public abstract class PlaywrightBase
{
    protected IPlaywright Playwright { get; private set; }
    protected IBrowser Browser { get; private set; }
    protected IPage Page { get; private set; }

    [SetUp]
    public async Task SetUp()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new() 
        {
            SlowMo = ConfigReader.Settings.Browser.SlowMo,
            Headless = ConfigReader.Settings.Browser.Headless,
            Timeout = ConfigReader.Settings.Browser.DefaultTimeout
        });
        Page = await Browser.NewPageAsync(new()
        {
            BaseURL = "https://www.saucedemo.com"
        });

        Playwright.Selectors.SetTestIdAttribute("data-test");
    }

    [TearDown]
    public async Task TearDown()
    {
        await Browser.DisposeAsync();
        Playwright.Dispose();
    }
}
