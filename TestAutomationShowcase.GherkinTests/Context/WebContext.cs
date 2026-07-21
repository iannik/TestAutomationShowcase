using Microsoft.Playwright;

namespace TestAutomationShowcase.GherkinTests.Context;

/// <summary>
/// Provides a context for managing browser and page instances for web automation tasks.
/// </summary>
/// <remarks>Initializes and disposes browser contexts and pages to support automated web interactions.</remarks>
public class WebContext
{
    public IBrowserContext? BrowserContext { get; private set; }
    public IPage? Page { get; private set; }
    public async Task InitAsync(IBrowser browser)
    {
        BrowserContext = await browser.NewContextAsync();
        Page = await BrowserContext.NewPageAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (BrowserContext != null)
        {
            await BrowserContext.CloseAsync();
            BrowserContext = null;
            Page = null;
        }
    }
}
