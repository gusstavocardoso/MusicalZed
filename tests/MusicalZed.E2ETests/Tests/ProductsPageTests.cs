namespace MusicalZed.E2ETests.Tests;

using FluentAssertions;
using Microsoft.Playwright.NUnit;
using MusicalZed.E2ETests.Helpers;
using MusicalZed.E2ETests.PageObjects;

[TestFixture]
[Category("E2E")]
public class ProductsPageTests : PageTest
{
    private ProductsPage _productsPage = null!;
    private string _baseUrl = null!;

    [SetUp]
    public void SetUp()
    {
        _baseUrl = Helpers.PlaywrightSetup.BaseUrl;
        _productsPage = new ProductsPage(Page, _baseUrl);
    }

    [Test]
    public async Task Products_ShouldDisplayProductGrid()
    {
        await _productsPage.GoToAsync();

        var count = await _productsPage.GetProductCountAsync();
        count.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task Products_ShouldShowFilterSidebar()
    {
        await _productsPage.GoToAsync();

        var visible = await _productsPage.IsFilterSidebarVisibleAsync();
        visible.Should().BeTrue();
    }

    [Test]
    public async Task Products_Search_ShouldFilterResults()
    {
        await _productsPage.GoToAsync();
        var totalBefore = await _productsPage.GetProductCountAsync();

        await _productsPage.SearchAsync("Guitarra");
        var filtered = await _productsPage.GetDisplayedProductCountAsync();

        filtered.Should().BeGreaterThan(0);
        filtered.Should().BeLessThanOrEqualTo(totalBefore);
    }

    [Test]
    public async Task Products_Search_WithNoResults_ShouldShowMessage()
    {
        await _productsPage.GoToAsync();
        await _productsPage.SearchAsync("xyzqwerty123nonexistent");

        var noResults = await _productsPage.IsNoResultsMessageVisibleAsync();
        noResults.Should().BeTrue();
    }

    [Test]
    public async Task Products_ClickProduct_ShouldNavigateToDetail()
    {
        await _productsPage.GoToAsync();
        await _productsPage.ClickFirstProductAsync();

        await Page.WaitForURLAsync(new System.Text.RegularExpressions.Regex("/produto/\\d+"));
        Page.Url.Should().Contain("/produto/");
    }

    [Test]
    public async Task Products_PageTitle_ShouldContainMusicalZed()
    {
        await _productsPage.GoToAsync();
        var title = await _productsPage.GetTitleAsync();
        title.Should().Contain("Musical Zed");
    }
}
