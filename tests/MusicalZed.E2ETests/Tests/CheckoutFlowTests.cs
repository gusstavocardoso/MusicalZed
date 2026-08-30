namespace MusicalZed.E2ETests.Tests;

using FluentAssertions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
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
    public void SetUp()
    {
        _baseUrl = Helpers.PlaywrightSetup.BaseUrl;
        _homePage = new HomePage(Page, _baseUrl);
        _cartPage = new CartPage(Page, _baseUrl);
        _checkoutPage = new CheckoutPage(Page, _baseUrl);
    }

    /// <summary>
    /// Navega todo o fluxo home → carrinho → checkout usando APENAS cliques internos
    /// do Blazor para preservar o circuito SignalR e o SessionId do CartStateService.
    ///
    /// Cada PageTest (NUnit) recebe uma nova Page/BrowserContext, então cada teste
    /// começa com um único GoToAsync na home para estabelecer o circuito inicial.
    /// Todas as navegações subsequentes usam cliques para manter o mesmo SessionId.
    /// </summary>
    private async Task NavigateToCheckoutWithItemAsync()
    {
        // Passo 1: Estabelece o circuito Blazor (único GoToAsync por teste)
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();

        // Passo 2: Adiciona produto ao carrinho
        await _homePage.AddFirstProductToCartAsync();

        // Passo 3: Aguarda toast — confirma que o item foi persistido no BD
        //          para o SessionId deste circuito
        await Page.Locator(".mz-toast").WaitForAsync(new() { Timeout = 10000 });
        await Page.WaitForTimeoutAsync(400);

        // Passo 4: Navega ao carrinho via CLIQUE no navbar (enhanced navigation —
        //          preserva circuito Blazor e SessionId)
        await _cartPage.NavigateViaClickAsync();

        // Passo 5: Clica em "Finalizar Pedido" no carrinho
        //          Cart.razor usa Nav.NavigateTo("/checkout") — mesma sessão
        await _cartPage.ClickCheckoutAsync();

        // Aguarda a página de checkout carregar
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/checkout"),
            new() { Timeout = 10000 });
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Test]
    public async Task Checkout_EmptyForm_ShouldShowError()
    {
        await NavigateToCheckoutWithItemAsync();

        // Tenta confirmar sem preencher o formulário
        await _checkoutPage.ClickConfirmAsync();

        var hasError = await _checkoutPage.IsErrorVisibleAsync();
        hasError.Should().BeTrue();
    }

    [Test]
    public async Task Checkout_WithValidData_ShouldRedirectToConfirmation()
    {
        await NavigateToCheckoutWithItemAsync();

        await _checkoutPage.FillCustomerDataAsync(
            "Carlos Teste", "carlos@teste.com", "(11) 91234-5678");
        await _checkoutPage.FillAddressAsync(
            "Av. Paulista, 1578", "São Paulo", "SP", "01310-200");
        await _checkoutPage.SelectPixPaymentAsync();
        await _checkoutPage.ClickConfirmAsync();

        await Page.WaitForURLAsync(
            new System.Text.RegularExpressions.Regex("/pedido-confirmado/\\d+"),
            new() { Timeout = 15000 });

        Page.Url.Should().Contain("/pedido-confirmado/");
    }

    [Test]
    public async Task OrderConfirmation_ShouldShowSuccessPage()
    {
        await NavigateToCheckoutWithItemAsync();

        await _checkoutPage.FillCustomerDataAsync(
            "Paula Silva", "paula@teste.com", "(21) 99876-5432");
        await _checkoutPage.FillAddressAsync(
            "Rua das Flores, 42", "Rio de Janeiro", "RJ", "20000-000");
        await _checkoutPage.SelectPixPaymentAsync();
        await _checkoutPage.ClickConfirmAsync();

        await Page.WaitForURLAsync(
            new System.Text.RegularExpressions.Regex("/pedido-confirmado/\\d+"),
            new() { Timeout = 15000 });

        var confirmationPage = new OrderConfirmationPage(Page, _baseUrl);
        var isSuccess = await confirmationPage.IsSuccessIconVisibleAsync();
        isSuccess.Should().BeTrue();
    }
}
