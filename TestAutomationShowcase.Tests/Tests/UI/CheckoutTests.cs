using Allure.NUnit;
using Allure.NUnit.Attributes;
using Microsoft.Playwright;
using TestAutomationShowcase.Configuration;
using TestAutomationShowcase.Core.PageObjects.SauceDemo;
using TestAutomationShowcase.Tests.Helpers;

namespace TestAutomationShowcase.Tests.Tests.UI;

/// <summary>
/// Contains UI tests for the checkout process in the SauceDemo application.
/// </summary>
/// <remarks>Tests include valid checkout scenarios and error handling for missing shipping details.</remarks>
[TestFixture]
[Parallelizable(ParallelScope.Self)]
[Category("UI")]
[AllureNUnit]
[AllureFeature("SauceDemo UI")]
[AllureSuite("Checkout UI Playwright Tests")]
public class CheckoutTests : PlaywrightBase
{
    private LoginPage _loginPage;
    private ProductsPage _productsPage;
    private CartPage _cartPage;
    private CheckoutPage _checkoutPage;

    [SetUp]
    public async Task SetUpPages()
    {
        _loginPage = new LoginPage(Page);
        _productsPage = new ProductsPage(Page);
        _cartPage = new CartPage(Page);
        _checkoutPage = new CheckoutPage(Page);

        await _loginPage.GoToAsync();
        await _loginPage.LoginAsync(ConfigReader.Settings.SauceDemoCredentialsStandard.Username, ConfigReader.Settings.SauceDemoCredentialsStandard.Password);
        await _productsPage.AddFirstItemToCartAsync();
        await _productsPage.GoToCartAsync();
        await _cartPage.ProceedToCheckoutAsync();
    }

    [Test]
    public async Task ValidCheckout_ShowsConfirmation()
    {
        await _checkoutPage.FillShippingDetailsAsync("John", "Doe", "12345");
        await _checkoutPage.ContinueAsync();
        await _checkoutPage.FinishAsync();

        await Assertions.Expect(_checkoutPage.Confirmation).ToHaveTextAsync("Thank you for your order!");
    }

    [Test]
    public async Task CheckoutWithoutFirstName_ShowsError()
    {
        await _checkoutPage.FillShippingDetailsAsync(string.Empty, "Doe", "12345");
        await _checkoutPage.ContinueAsync();

        await Assertions.Expect(_checkoutPage.Error).ToBeVisibleAsync();
    }

    [Test]
    public async Task CheckoutWithoutPostalCode_ShowsError()
    {
        await _checkoutPage.FillShippingDetailsAsync("John", "Doe", string.Empty);
        await _checkoutPage.ContinueAsync();

        await Assertions.Expect(_checkoutPage.Error).ToBeVisibleAsync();
    }
}
