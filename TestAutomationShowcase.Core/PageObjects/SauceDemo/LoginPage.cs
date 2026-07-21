using Microsoft.Playwright;
using TestAutomationShowcase.Configuration;

namespace TestAutomationShowcase.Core.PageObjects.SauceDemo;

public class LoginPage(IPage page)
{
    private ILocator UsernameInput => page.GetByTestId("username");
    private ILocator PasswordInput => page.GetByTestId("password");
    private ILocator LoginButton => page.GetByTestId("login-button");
    private ILocator ErrorMessage => page.GetByTestId("error");

    public async Task GoToAsync() => await page.GotoAsync(ConfigReader.Settings.UiBaseUrl);

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public ILocator Error => ErrorMessage;
}
