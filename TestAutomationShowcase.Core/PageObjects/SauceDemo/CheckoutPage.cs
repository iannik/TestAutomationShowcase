using Microsoft.Playwright;

namespace TestAutomationShowcase.Core.PageObjects.SauceDemo;

public class CheckoutPage(IPage page)
{
    private ILocator FirstNameInput => page.GetByTestId("firstName");
    private ILocator LastNameInput => page.GetByTestId("lastName");
    private ILocator PostalCodeInput => page.GetByTestId("postalCode");
    private ILocator ContinueButton => page.GetByTestId("continue");
    private ILocator FinishButton => page.GetByTestId("finish");
    private ILocator ConfirmationHeader => page.GetByTestId("complete-header");
    private ILocator ErrorMessage => page.GetByTestId("error");

    public async Task FillShippingDetailsAsync(string firstName, string lastName, string postalCode)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await PostalCodeInput.FillAsync(postalCode);
    }

    public async Task ContinueAsync() => await ContinueButton.ClickAsync();

    public async Task FinishAsync() => await FinishButton.ClickAsync();

    public ILocator Confirmation => ConfirmationHeader;
    public ILocator Error => ErrorMessage;
}
