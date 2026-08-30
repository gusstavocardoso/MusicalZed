namespace MusicalZed.E2ETests.Tests;

using FluentAssertions;
using Microsoft.Playwright.NUnit;
using MusicalZed.E2ETests.Helpers;
using MusicalZed.E2ETests.PageObjects;

[TestFixture]
[Category("E2E")]
public class HomePageTests : PageTest
{
    private HomePage _homePage = null!;
    private string _baseUrl = null!;

    [SetUp]
    public void SetUp()
    {
        _baseUrl = Helpers.PlaywrightSetup.BaseUrl;
        _homePage = new HomePage(Page, _baseUrl);
    }

    [Test]
    public async Task Home_ShouldDisplayHeroSection()
    {
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();

        var isVisible = await _homePage.IsHeroVisibleAsync();
        isVisible.Should().BeTrue();
    }

    [Test]
    public async Task Home_ShouldHaveCorrectPageTitle()
    {
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();

        var title = await _homePage.GetTitleAsync();
        title.Should().Contain("Musical Zed");
    }

    [Test]
    public async Task Home_HeroTitle_ShouldContainExpectedText()
    {
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();

        var title = await _homePage.GetHeroTitleAsync();
        title.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task Home_ShouldDisplayCategories()
    {
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();

        var count = await _homePage.GetCategoryCountAsync();
        count.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task Home_ShouldDisplayFeaturedProducts()
    {
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();

        var count = await _homePage.GetFeaturedProductCountAsync();
        count.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task Home_ClickingProduct_ShouldNavigateToDetail()
    {
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();

        await _homePage.ClickFirstProductAsync();
        await Page.WaitForURLAsync(new System.Text.RegularExpressions.Regex("/produto/\\d+"));

        Page.Url.Should().Contain("/produto/");
    }

    [Test]
    public async Task Home_ClickingCategory_ShouldNavigateToProducts()
    {
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();

        await _homePage.ClickCategoryAsync(0);
        await Page.WaitForURLAsync(new System.Text.RegularExpressions.Regex("/produtos"));

        Page.Url.Should().Contain("/produtos");
    }
}
