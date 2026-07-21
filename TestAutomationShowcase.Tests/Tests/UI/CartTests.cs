using Allure.NUnit;
using Allure.NUnit.Attributes;
using Microsoft.Playwright;
using TestAutomationShowcase.Configuration;
using TestAutomationShowcase.Core.PageObjects.SauceDemo;
using TestAutomationShowcase.Tests.Helpers;

namespace TestAutomationShowcase.Tests.Tests.UI;

/// <summary>
/// Contains UI tests for cart functionality in the SauceDemo application using Playwright.
/// </summary>
/// <remarks>Includes tests for adding and removing items from the cart and verifying the cart badge count.</remarks>
[TestFixture]
[Parallelizable(ParallelScope.Self)]
[Category("UI")]
[AllureNUnit]
[AllureFeature("SauceDemo UI")]
[AllureSuite("Cart UI Playwright Tests")]
public class CartTests : PlaywrightBase
{
    private LoginPage _loginPage;
    private ProductsPage _productsPage;
    private CartPage _cartPage;

    [SetUp]
    public async Task SetUpPages()
    {
        _loginPage = new LoginPage(Page);
        _productsPage = new ProductsPage(Page);
        _cartPage = new CartPage(Page);

        await _loginPage.GoToAsync();
        await _loginPage.LoginAsync(ConfigReader.Settings.SauceDemoCredentialsStandard.Username, ConfigReader.Settings.SauceDemoCredentialsStandard.Password);
    }

    [Test]
    public async Task AddItemToCart_UpdatesCartBadge()
    {
        await _productsPage.AddFirstItemToCartAsync();

        await Assertions.Expect(_productsPage.CartBadgeCount).ToHaveTextAsync("1");
    }

    [Test]
    public async Task AddItemToCart_ItemAppearsInCart()
    {
        await _productsPage.AddFirstItemToCartAsync();
        await _productsPage.GoToCartAsync();

        await Assertions.Expect(_cartPage.Items).ToHaveCountAsync(1);
    }

    [Test]
    public async Task RemoveItemFromCart_CartBecomesEmpty()
    {
        await _productsPage.AddFirstItemToCartAsync();
        await _productsPage.GoToCartAsync();
        await _cartPage.RemoveFirstItemAsync();

        await Assertions.Expect(_cartPage.Items).ToHaveCountAsync(0);
    }
}