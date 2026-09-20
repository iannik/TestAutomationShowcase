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
        await Page.Context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TearDown]
    public async Task TearDown()
    {
        var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
        var testFailed = testStatus == NUnit.Framework.Interfaces.TestStatus.Failed;

        try
        {
            if (testFailed)
            {
                var artifactDirectory = Path.Combine(
                    TestContext.CurrentContext.WorkDirectory,
                    "playwright-artifacts");
                Directory.CreateDirectory(artifactDirectory);

                await Page.ScreenshotAsync(new()
                {
                    Path = Path.Combine(artifactDirectory, $"{Guid.NewGuid():N}.png"),
                    FullPage = true
                });

                await Page.Context.Tracing.StopAsync(new()
                {
                    Path = Path.Combine(artifactDirectory, $"{Guid.NewGuid():N}.zip")
                });
            }
            else
            {
                await Page.Context.Tracing.StopAsync();
            }
        }
        finally
        {
            await Browser.DisposeAsync();
            Playwright.Dispose();
        }
    }
}
