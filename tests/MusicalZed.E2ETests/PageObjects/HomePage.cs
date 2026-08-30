namespace MusicalZed.E2ETests.PageObjects;

using Microsoft.Playwright;

public class HomePage : BasePage
{
    // Selectors
    private const string HeroSection = ".mz-hero";
    private const string HeroTitle = ".mz-hero h1";
    private const string CategoryCards = ".mz-category-card";
    private const string ProductCards = ".mz-product-card";
    private const string ToastMessage = ".mz-toast .toast-body";
    private const string AddToCartButtons = ".mz-product-card .btn-mz";
    private const string CartBadge = ".navbar .badge";
    private const string ViewAllButton = "a.btn-mz-outline";

    public HomePage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task GoToAsync() => await NavigateAsync("/");

    public async Task<bool> IsHeroVisibleAsync() =>
        await IsElementVisibleAsync(HeroSection);

    public async Task<string> GetHeroTitleAsync() =>
        await Page.Locator(HeroTitle).InnerTextAsync();

    public async Task<int> GetCategoryCountAsync() =>
        await Page.Locator(CategoryCards).CountAsync();

    public async Task<int> GetFeaturedProductCountAsync() =>
        await Page.Locator(ProductCards).CountAsync();

    public async Task<string> GetFirstProductNameAsync()
    {
        await Page.Locator(ProductCards).First.WaitForAsync();
        return await Page.Locator($"{ProductCards} .product-name").First.InnerTextAsync();
    }

    public async Task AddFirstProductToCartAsync()
    {
        var btn = Page.Locator(AddToCartButtons).First;
        await btn.WaitForAsync();
        await btn.ClickAsync();
    }

    public async Task<string?> GetToastMessageAsync()
    {
        try
        {
            await Page.Locator(ToastMessage).WaitForAsync(new() { Timeout = 5000 });
            return await Page.Locator(ToastMessage).InnerTextAsync();
        }
        catch { return null; }
    }

    public async Task<int?> GetCartBadgeCountAsync()
    {
        try
        {
            var badge = Page.Locator(CartBadge);
            if (!await badge.IsVisibleAsync()) return null;
            var text = await badge.InnerTextAsync();
            return int.TryParse(text.Trim(), out var n) ? n : null;
        }
        catch { return null; }
    }

    public async Task ClickCategoryAsync(int index = 0) =>
        await Page.Locator(CategoryCards).Nth(index).ClickAsync();

    public async Task ClickFirstProductAsync() =>
        await Page.Locator(ProductCards).First.ClickAsync();

    public async Task NavigateToCartAsync() =>
        await Page.Locator("a[href='/carrinho']").ClickAsync();
}
