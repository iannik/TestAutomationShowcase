using Microsoft.Playwright;
using Reqnroll;
using TestAutomationShowcase.Core.PageObjects.SauceDemo;
using TestAutomationShowcase.GherkinTests.Context;

namespace TestAutomationShowcase.GherkinTests.StepDefinitions;

[Binding]
public class CartSteps
{
    private readonly WebContext _webContext;
    private CartPage _cartPage;

    public CartSteps(WebContext webContext)
    {
        _webContext = webContext;
        _cartPage = new CartPage(_webContext.Page);
    }

    /// <summary>
    /// Checks the number of items in the cart.
    /// </summary>
    /// <param name="count">Number of items</param>
    [Then("the cart should contain {int} item")]
    public async Task CheckCartItemsCount(int count)
    {
        await Assertions.Expect(_cartPage.Items).ToHaveCountAsync(count);
    }

    /// <summary>
    /// Removes the first item from the cart.
    /// </summary>
    [When("the user removes the first item from the cart")]
    public async Task RemoveFirstItemFromCart()
    {
        await _cartPage.RemoveFirstItemAsync();
    }

    /// <summary>
    /// Verifies that the cart is empty.
    /// </summary>
    [Then("the cart should be empty")]
    public async Task CheckCartIsEmpty()
    {
        await Assertions.Expect(_cartPage.Items).ToHaveCountAsync(0);
    }

    /// <summary>
    /// Initiates the checkout process for the user.
    /// </summary>
    [Given("the user proceeds to the checkout")]
    [When("the user proceeds to the checkout")]
    public async Task ProceedToCheckout()
    {
        await _cartPage.ProceedToCheckoutAsync();
    }
}
