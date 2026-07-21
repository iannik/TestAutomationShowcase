using Microsoft.Playwright;
using Reqnroll;
using TestAutomationShowcase.Core.PageObjects.SauceDemo;
using TestAutomationShowcase.GherkinTests.Context;

namespace TestAutomationShowcase.GherkinTests.StepDefinitions;

[Binding]
public class ProductsSteps
{
    private readonly WebContext _webContext;
    private ProductsPage _productsPage;
    public ProductsSteps(WebContext webContext)
    {
        _webContext = webContext;
        _productsPage = new ProductsPage(_webContext.Page);
    }

    /// <summary>
    /// Adds the first item to the cart on the products page.
    /// </summary>
    [Given("the user adds the first item to the cart")]
    [When("the user adds the first item to the cart")]
    public async Task AddFirstItemToCart()
    {
        await _productsPage.AddFirstItemToCartAsync();
    }

    /// <summary>
    /// Verifies that the cart badge displays the expected number of items.
    /// </summary>
    /// <param name="count">Number of items</param>
    [Then("the cart badge should show {int} item")]
    public async Task CheckCartBadgeCount(int count)
    {
        await Assertions.Expect(_productsPage.CartBadgeCount).ToHaveTextAsync(count.ToString());
    }

    /// <summary>
    /// Opens the cart page by clicking on the cart link.
    /// </summary>
    [Given("the user opens the cart")]
    [When("the user opens the cart")]
    public async Task OpenTheCart()
    {
        await _productsPage.GoToCartAsync();
    }
}
