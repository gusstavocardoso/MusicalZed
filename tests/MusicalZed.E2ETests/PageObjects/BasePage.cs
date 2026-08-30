namespace MusicalZed.E2ETests.PageObjects;

using Microsoft.Playwright;

public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly string BaseUrl;

    protected BasePage(IPage page, string baseUrl)
    {
        Page = page;
        BaseUrl = baseUrl;
    }

    public async Task WaitForLoadAsync() =>
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    public async Task<string> GetTitleAsync() => await Page.TitleAsync();

    public async Task<bool> IsElementVisibleAsync(string selector) =>
        await Page.IsVisibleAsync(selector);

    public async Task NavigateAsync(string path = "") =>
        await Page.GotoAsync($"{BaseUrl}{path}");
}
