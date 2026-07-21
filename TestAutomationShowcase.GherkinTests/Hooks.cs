using Microsoft.Playwright;
using Reqnroll;
using Reqnroll.BoDi;
using TestAutomationShowcase.Configuration;
using TestAutomationShowcase.GherkinTests;
using TestAutomationShowcase.GherkinTests.Context;

[Binding]
public class Hooks
{
    private readonly IObjectContainer _container;
    private WebContext WebContext => _container.Resolve<WebContext>();
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;

    public Hooks(IObjectContainer container, ScenarioContext scenarioContext)
    {
        _container = container;
    }

    [BeforeScenario(Order = 0)]
    public void BuildServices(IObjectContainer container)
    {
        var servicesBuilder = new ServicesBuilder(container);
        servicesBuilder.Build();
    }

    [BeforeScenario("UI", Order = 1)]
    public async Task InitWebContext()
    {
        _playwright = await Playwright.CreateAsync();
        _playwright.Selectors.SetTestIdAttribute("data-test");

        _browser = await _playwright.Chromium.LaunchAsync(new()
        {
            SlowMo = ConfigReader.Settings.Browser.SlowMo,
            Headless = ConfigReader.Settings.Browser.Headless,
            Timeout = ConfigReader.Settings.Browser.DefaultTimeout
        });

        await WebContext.InitAsync(_browser);
    }


    [AfterScenario("UI", Order = 0)]
    public async Task CloseWebContext()
    {
        await WebContext.DisposeAsync();
    }

    [AfterScenario("UI", Order = 1)]
    public static void CloseBrowser(IObjectContainer container)
    {
        _browser?.CloseAsync();
        _playwright?.Dispose();
    }

}