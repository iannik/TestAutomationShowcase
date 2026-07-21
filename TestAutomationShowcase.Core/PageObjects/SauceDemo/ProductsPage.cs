using Microsoft.Playwright;

namespace TestAutomationShowcase.Core.PageObjects.SauceDemo;

public class ProductsPage(IPage page)
{
    private ILocator ProductItems => page.GetByTestId("inventory_item");
    private ILocator AddToCartButtons => page.GetByRole(AriaRole.Button).GetByText("Add to cart");
    private ILocator CartBadge => page.GetByTestId("shopping-cart-badge");
    private ILocator CartLink => page.GetByTestId("shopping-cart-link");

    public async Task AddFirstItemToCartAsync() => await AddToCartButtons.First.ClickAsync();

    public async Task AddItemToCartByNameAsync(string name) => await page.GetByTestId($"add-to-cart-{name}").ClickAsync();

    public async Task GoToCartAsync() => await CartLink.ClickAsync();

    public ILocator Products => ProductItems;
    public ILocator CartBadgeCount => CartBadge;
}
