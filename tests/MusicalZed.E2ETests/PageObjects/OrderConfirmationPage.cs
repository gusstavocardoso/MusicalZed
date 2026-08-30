namespace MusicalZed.E2ETests.PageObjects;

using Microsoft.Playwright;

public class OrderConfirmationPage : BasePage
{
    private const string SuccessIcon = ".order-icon .fa-circle-check";
    private const string SuccessTitle = "h2.text-success";
    private const string OrderIdBadge = ".badge.bg-primary";
    private const string CustomerName = "h2 + p strong";
    private const string ContinueShoppingBtn = "a.btn-mz-outline";
    private const string BackHomeBtn = "a.btn-mz";

    public OrderConfirmationPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task<bool> IsSuccessIconVisibleAsync() =>
        await IsElementVisibleAsync(SuccessIcon);

    public async Task<string> GetSuccessTitleAsync() =>
        await Page.Locator(SuccessTitle).InnerTextAsync();

    public async Task<string> GetOrderIdTextAsync() =>
        await Page.Locator(OrderIdBadge).InnerTextAsync();

    public async Task<bool> IsBackHomeButtonVisibleAsync() =>
        await IsElementVisibleAsync(BackHomeBtn);
}
