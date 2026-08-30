namespace MusicalZed.E2ETests.Tests;

using FluentAssertions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using MusicalZed.E2ETests.PageObjects;

[TestFixture]
[Category("E2E")]
public class CartFlowTests : PageTest
{
    private HomePage _homePage = null!;
    private CartPage _cartPage = null!;
    private string _baseUrl = null!;

    [SetUp]
    public void SetUp()
    {
        _baseUrl = Helpers.PlaywrightSetup.BaseUrl;
        _homePage = new HomePage(Page, _baseUrl);
        _cartPage = new CartPage(Page, _baseUrl);
    }

    [Test]
    public async Task Cart_WhenEmpty_ShouldShowEmptyMessage()
    {
        // GoToAsync cria novo circuito Blazor — correto para testar carrinho vazio
        await _cartPage.GoToAsync();
        await _cartPage.WaitForCartLoadedAsync();

        var isEmpty = await _cartPage.IsEmptyAsync();
        isEmpty.Should().BeTrue();
    }

    [Test]
    public async Task Cart_AfterAddingProduct_ShouldShowItem()
    {
        // 1. Inicia na home (estabelece o circuito Blazor)
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();

        // 2. Adiciona produto
        await _homePage.AddFirstProductToCartAsync();

        // 3. Aguarda toast de confirmação — garante que o item foi gravado no BD
        await Page.Locator(".mz-toast").WaitForAsync(new() { Timeout = 8000 });
        await Page.WaitForTimeoutAsync(400);

        // 4. Navega ao carrinho via CLIQUE (preserva o circuito Blazor e SessionId)
        await _cartPage.NavigateViaClickAsync();

        var count = await _cartPage.GetItemCountAsync();
        count.Should().Be(1);
    }

    [Test]
    public async Task Cart_CheckoutButton_ShouldNavigateToCheckout()
    {
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();
        await _homePage.AddFirstProductToCartAsync();

        await Page.Locator(".mz-toast").WaitForAsync(new() { Timeout = 8000 });
        await Page.WaitForTimeoutAsync(400);

        // Navega ao carrinho via clique (mesmo circuito)
        await _cartPage.NavigateViaClickAsync();

        // Clica em "Finalizar Pedido" — usa NavigationManager interno do Blazor
        await _cartPage.ClickCheckoutAsync();

        await Page.WaitForURLAsync(new System.Text.RegularExpressions.Regex("/checkout"),
            new() { Timeout = 10000 });
        Page.Url.Should().Contain("/checkout");
    }

    [Test]
    public async Task Cart_ClearCart_ShouldShowEmptyMessage()
    {
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();
        await _homePage.AddFirstProductToCartAsync();

        await Page.Locator(".mz-toast").WaitForAsync(new() { Timeout = 8000 });
        await Page.WaitForTimeoutAsync(400);

        // Navega ao carrinho via clique (mesmo circuito)
        await _cartPage.NavigateViaClickAsync();

        await _cartPage.ClearCartAsync();
        await _cartPage.WaitForCartLoadedAsync();

        var isEmpty = await _cartPage.IsEmptyAsync();
        isEmpty.Should().BeTrue();
    }
}
