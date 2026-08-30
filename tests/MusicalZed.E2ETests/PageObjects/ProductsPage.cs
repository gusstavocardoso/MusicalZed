namespace MusicalZed.E2ETests.PageObjects;

using Microsoft.Playwright;

public class ProductsPage : BasePage
{
    private const string ProductCards = ".mz-product-card";
    private const string SearchInput = "input[placeholder='Nome, marca...']";
    private const string FilterCard = ".filter-card";
    private const string ProductCount = ".badge.bg-secondary";
    private const string NoResultsMessage = ".text-center .text-muted";
    private const string AddToCartButtons = ".mz-product-card .btn-mz";

    public ProductsPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task GoToAsync(int? categoryId = null)
    {
        var url = categoryId.HasValue ? $"/produtos?categoriaId={categoryId}" : "/produtos";
        await NavigateAsync(url);
        await WaitForLoadAsync();
    }

    public async Task<int> GetProductCountAsync()
    {
        await Page.Locator(ProductCards).First.WaitForAsync(new() { Timeout = 10000 });
        return await Page.Locator(ProductCards).CountAsync();
    }

    public async Task<bool> IsFilterSidebarVisibleAsync() =>
        await IsElementVisibleAsync(FilterCard);

    public async Task SearchAsync(string query)
    {
        var input = Page.Locator(SearchInput);
        await input.FillAsync(query);
        await Page.WaitForTimeoutAsync(600); // debounce
        await WaitForLoadAsync();
    }

    public async Task<int> GetDisplayedProductCountAsync() =>
        await Page.Locator(ProductCards).CountAsync();

    public async Task ClickFirstProductAsync() =>
        await Page.Locator(ProductCards).First.ClickAsync();

    public async Task AddFirstProductToCartAsync() =>
        await Page.Locator(AddToCartButtons).First.ClickAsync();

    public async Task<bool> IsNoResultsMessageVisibleAsync() =>
        await IsElementVisibleAsync(NoResultsMessage);
}
