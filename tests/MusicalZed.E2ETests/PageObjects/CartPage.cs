namespace MusicalZed.E2ETests.PageObjects;

using Microsoft.Playwright;

public class CartPage : BasePage
{
    private const string CartItems = ".cart-item-card";
    private const string EmptyCartMessage = ".fa-cart-shopping.fa-4x";
    private const string CheckoutButton = ".btn-mz:has-text('Finalizar')";
    private const string ClearCartButton = "button:has-text('Limpar carrinho')";
    private const string TotalAmount = ".cart-summary .fw-bold.fs-5 [style*='mz-accent']";
    private const string IncreaseQtyBtns = ".cart-item-card button:has-text('+')";
    private const string RemoveButtons = ".cart-item-card .text-danger";

    public CartPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task GoToAsync() => await NavigateAsync("/carrinho");

    public async Task<int> GetItemCountAsync() =>
        await Page.Locator(CartItems).CountAsync();

    public async Task<bool> IsEmptyAsync() =>
        await IsElementVisibleAsync(EmptyCartMessage);

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

    public async Task IncreaseFirstItemQtyAsync() =>
        await Page.Locator(IncreaseQtyBtns).First.ClickAsync();

    public async Task RemoveFirstItemAsync() =>
        await Page.Locator(RemoveButtons).First.ClickAsync();
}
