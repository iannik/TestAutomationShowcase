using Microsoft.Playwright;

namespace TestAutomationShowcase.Core.PageObjects.SauceDemo;

public class CartPage(IPage page)
{
    private ILocator CartItems => page.GetByTestId("inventory-item");
    private ILocator CheckoutButton => page.GetByTestId("checkout");
    private ILocator RemoveButtons => page.GetByRole(AriaRole.Button).GetByText("Remove");

    public async Task ProceedToCheckoutAsync() => await CheckoutButton.ClickAsync();

    public async Task RemoveFirstItemAsync() => await RemoveButtons.First.ClickAsync();

    public ILocator Items => CartItems;
}
