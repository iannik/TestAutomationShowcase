using Microsoft.Playwright;
using Reqnroll;
using TestAutomationShowcase.Core.Models;
using TestAutomationShowcase.Core.PageObjects.SauceDemo;
using TestAutomationShowcase.GherkinTests.Context;

namespace TestAutomationShowcase.GherkinTests.StepDefinitions;

[Binding]
public class CheckoutSteps
{
    private readonly WebContext _webContext;
    private CheckoutPage _checkoutPage;

    public CheckoutSteps(WebContext webContext)
    {
        _webContext = webContext;
        _checkoutPage = new CheckoutPage(_webContext.Page);
    }

    /// <summary>
    /// Fills in the shipping details on the checkout page using the provided table data.
    /// </summary>
    /// <param name="table">Gherkin table with shipping details</param>
    /// <example>
    /// When the user enters the shipping details:
    ///  | FirstName | LastName | PostalCode |
    ///  | John      | Doe      |      12345 |
    /// </example>
    [When("the user enters the shipping details:")]
    public async Task EnterShippingDetails(Table table)
    {
        var shippingDetails = table.CreateInstance<ShippingDetails>();

        await _checkoutPage.FillShippingDetailsAsync(shippingDetails.FirstName, 
                                                     shippingDetails.LastName, 
                                                     shippingDetails.PostalCode);
    }

    /// <summary>
    /// Continues to the checkout overview page by clicking the continue button on the checkout page.
    /// </summary>
    [When("the user continues to the checkout overview")]
    public async Task ContinueToCheckoutOverview()
    {
        await _checkoutPage.ContinueAsync();
    }

    /// <summary>
    /// Completes the checkout process by clicking the finish button on the checkout overview page.
    /// </summary>
    [When("the user completes the checkout")]
    public async Task CompleteCheckout()
    {
        await _checkoutPage.FinishAsync();
    }

    /// <summary>
    /// Verifies that the checkout confirmation message is displayed on the page after completing the checkout process.
    /// </summary>
    [Then("the checkout confirmation message should be displayed")]
    public async Task CheckCheckoutConfirmation()
    {
        await Assertions.Expect(_checkoutPage.Confirmation).ToBeVisibleAsync();
    }

    /// <summary>
    /// Verifies that the checkout error message is displayed on the page when there is an issue with the checkout process.
    /// </summary>
    [Then("the checkout error message should be displayed")]
    public async Task CheckCheckoutErrorIsVisible()
    {
        await Assertions.Expect(_checkoutPage.Error).ToBeVisibleAsync();
    }

    /// <summary>
    /// Verifies that the checkout error message matches the expected text provided.
    /// </summary>
    /// <param name="message">Expected message text</param>
    [Then("the checkout error message should read {string}")]
    public async Task CheckCheckoutErrorMessage(string message)
    {
        await Assertions.Expect(_checkoutPage.Error).ToHaveTextAsync(message);
    }
}
