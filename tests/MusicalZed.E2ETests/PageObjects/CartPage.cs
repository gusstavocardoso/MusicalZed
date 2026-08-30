namespace MusicalZed.E2ETests.PageObjects;

using Microsoft.Playwright;

public class CartPage : BasePage
{
    private const string CartItems = ".cart-item-card";
    private const string EmptyCartMessage = ".fa-cart-shopping.fa-4x";
    private const string CheckoutButton = ".btn-mz:has-text('Finalizar')";
    private const string ClearCartButton = "button:has-text('Limpar carrinho')";
    private const string NavCartLink = "a[href='/carrinho']";

    public CartPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    /// <summary>
    /// Navegação direta via URL — cria NOVO circuito Blazor.
    /// Usar apenas quando o carrinho precisa estar vazio (teste de carrinho vazio).
    /// </summary>
    public async Task GoToAsync() => await NavigateAsync("/carrinho");

    /// <summary>
    /// Navegação via clique no link da navbar — PRESERVA o circuito Blazor existente
    /// e mantém o SessionId do CartStateService. Usar após interações na página atual.
    /// </summary>
    public async Task NavigateViaClickAsync()
    {
        await Page.Locator(NavCartLink).First.ClickAsync();
        await WaitForCartLoadedAsync();
    }

    /// <summary>
    /// Aguarda o carrinho terminar de carregar (spinner desaparece ou conteúdo aparece).
    /// </summary>
    public async Task WaitForCartLoadedAsync()
    {
        // Aguarda o spinner sumir ou o conteúdo do carrinho aparecer
        try
        {
            await Page.Locator(".mz-spinner").WaitForAsync(new()
            {
                State = WaitForSelectorState.Hidden,
                Timeout = 8000
            });
        }
        catch { /* Spinner pode não aparecer se Blazor renderizar rapidamente */ }

        await Page.WaitForTimeoutAsync(500);
    }

    public async Task<int> GetItemCountAsync()
    {
        await WaitForCartLoadedAsync();
        return await Page.Locator(CartItems).CountAsync();
    }

    public async Task<bool> IsEmptyAsync()
    {
        await WaitForCartLoadedAsync();
        return await IsElementVisibleAsync(EmptyCartMessage);
    }

    public async Task ClickCheckoutAsync() =>
        await Page.Locator(CheckoutButton).ClickAsync();

    public async Task ClearCartAsync()
    {
        var btn = Page.Locator(ClearCartButton);
        if (await btn.IsVisibleAsync())
            await btn.ClickAsync();
    }

    public async Task<bool> IsCheckoutButtonVisibleAsync() =>
        await IsElementVisibleAsync(CheckoutButton);
}
