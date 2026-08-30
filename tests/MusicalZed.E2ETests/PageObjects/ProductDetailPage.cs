namespace MusicalZed.E2ETests.PageObjects;

using Microsoft.Playwright;

public class ProductDetailPage : BasePage
{
    private const string ProductTitle = "h1.h2, h1.fw-bold";
    private const string ProductPrice = ".display-6.fw-bold, [style*='mz-accent']";
    private const string AddToCartBtn = ".btn-mz";
    private const string IncreaseQtyBtn = "button:has-text('+')";
    private const string DecreaseQtyBtn = "button:has-text('−')";
    private const string QuantityDisplay = ".fw-bold.border-start";
    private const string SuccessAlert = ".alert-success";
    private const string BreadcrumbNav = "nav[aria-label='breadcrumb']";

    public ProductDetailPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task GoToAsync(int productId) =>
        await NavigateAsync($"/produto/{productId}");

    public async Task<string> GetProductNameAsync()
    {
        await Page.Locator(ProductTitle).First.WaitForAsync();
        return await Page.Locator(ProductTitle).First.InnerTextAsync();
    }

    public async Task<bool> IsBreadcrumbVisibleAsync() =>
        await IsElementVisibleAsync(BreadcrumbNav);

    public async Task AddToCartAsync() =>
        await Page.Locator(AddToCartBtn).First.ClickAsync();

    public async Task IncreaseQuantityAsync() =>
        await Page.Locator(IncreaseQtyBtn).ClickAsync();

    public async Task<bool> IsSuccessAlertVisibleAsync()
    {
        try
        {
            await Page.Locator(SuccessAlert).WaitForAsync(new() { Timeout = 5000 });
            return true;
        }
        catch { return false; }
    }
}
