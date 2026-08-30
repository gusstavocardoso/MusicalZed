namespace MusicalZed.E2ETests.Tests;

using FluentAssertions;
using Microsoft.Playwright.NUnit;
using MusicalZed.E2ETests.Helpers;
using MusicalZed.E2ETests.PageObjects;

[TestFixture]
[Category("E2E")]
public class CheckoutFlowTests : PageTest
{
    private HomePage _homePage = null!;
    private CartPage _cartPage = null!;
    private CheckoutPage _checkoutPage = null!;
    private string _baseUrl = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        _baseUrl = Helpers.PlaywrightSetup.BaseUrl;
        _homePage = new HomePage(Page, _baseUrl);
        _cartPage = new CartPage(Page, _baseUrl);
        _checkoutPage = new CheckoutPage(Page, _baseUrl);

        // Add item to cart before checkout tests
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();
        await _homePage.AddFirstProductToCartAsync();
        await Page.WaitForTimeoutAsync(1000);
    }

    [Test]
    public async Task Checkout_EmptyForm_ShouldShowError()
    {
        await _checkoutPage.GoToAsync();
        await _checkoutPage.WaitForLoadAsync();
        await _checkoutPage.ClickConfirmAsync();

        var hasError = await _checkoutPage.IsErrorVisibleAsync();
        hasError.Should().BeTrue();
    }

    [Test]
    public async Task Checkout_WithValidData_ShouldRedirectToConfirmation()
    {
        await _checkoutPage.GoToAsync();
        await _checkoutPage.WaitForLoadAsync();

        await _checkoutPage.FillCustomerDataAsync(
            "Carlos Teste", "carlos@teste.com", "(11) 91234-5678");
        await _checkoutPage.FillAddressAsync(
            "Av. Paulista, 1578", "São Paulo", "SP", "01310-200");
        await _checkoutPage.SelectPixPaymentAsync();
        await _checkoutPage.ClickConfirmAsync();

        await Page.WaitForURLAsync(
            new System.Text.RegularExpressions.Regex("/pedido-confirmado/\\d+"),
            new() { Timeout = 10000 });

        Page.Url.Should().Contain("/pedido-confirmado/");
    }

    [Test]
    public async Task OrderConfirmation_ShouldShowSuccessPage()
    {
        await _checkoutPage.GoToAsync();
        await _checkoutPage.WaitForLoadAsync();

        await _checkoutPage.FillCustomerDataAsync(
            "Paula Silva", "paula@teste.com", "(21) 99876-5432");
        await _checkoutPage.FillAddressAsync(
            "Rua das Flores, 42", "Rio de Janeiro", "RJ", "20000-000");
        await _checkoutPage.SelectPixPaymentAsync();
        await _checkoutPage.ClickConfirmAsync();

        await Page.WaitForURLAsync(
            new System.Text.RegularExpressions.Regex("/pedido-confirmado/\\d+"),
            new() { Timeout = 10000 });

        var confirmationPage = new OrderConfirmationPage(Page, _baseUrl);
        var isSuccess = await confirmationPage.IsSuccessIconVisibleAsync();
        isSuccess.Should().BeTrue();
    }
}
