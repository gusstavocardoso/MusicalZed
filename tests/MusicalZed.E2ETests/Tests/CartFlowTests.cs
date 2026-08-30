namespace MusicalZed.E2ETests.Tests;

using FluentAssertions;
using Microsoft.Playwright.NUnit;
using MusicalZed.E2ETests.Helpers;
using MusicalZed.E2ETests.PageObjects;

[TestFixture]
[Category("E2E")]
public class CartFlowTests : PageTest
{
    private HomePage _homePage = null!;
    private CartPage _cartPage = null!;
    private string _baseUrl = null!;

    [SetUp]
    public void SetUp()
    {
        _baseUrl = Helpers.PlaywrightSetup.BaseUrl;
        _homePage = new HomePage(Page, _baseUrl);
        _cartPage = new CartPage(Page, _baseUrl);
    }

    [Test]
    public async Task Cart_WhenEmpty_ShouldShowEmptyMessage()
    {
        await _cartPage.GoToAsync();
        await _cartPage.WaitForLoadAsync();

        var isEmpty = await _cartPage.IsEmptyAsync();
        isEmpty.Should().BeTrue();
    }

    [Test]
    public async Task Cart_AfterAddingProduct_ShouldShowItem()
    {
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();
        await _homePage.AddFirstProductToCartAsync();
        await Page.WaitForTimeoutAsync(1000);

        await _cartPage.GoToAsync();
        await _cartPage.WaitForLoadAsync();

        var count = await _cartPage.GetItemCountAsync();
        count.Should().Be(1);
    }

    [Test]
    public async Task Cart_CheckoutButton_ShouldNavigateToCheckout()
    {
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();
        await _homePage.AddFirstProductToCartAsync();
        await Page.WaitForTimeoutAsync(1000);

        await _cartPage.GoToAsync();
        await _cartPage.WaitForLoadAsync();
        await _cartPage.ClickCheckoutAsync();

        await Page.WaitForURLAsync(new System.Text.RegularExpressions.Regex("/checkout"));
        Page.Url.Should().Contain("/checkout");
    }

    [Test]
    public async Task Cart_ClearCart_ShouldShowEmptyMessage()
    {
        await _homePage.GoToAsync();
        await _homePage.WaitForLoadAsync();
        await _homePage.AddFirstProductToCartAsync();
        await Page.WaitForTimeoutAsync(1000);

        await _cartPage.GoToAsync();
        await _cartPage.WaitForLoadAsync();
        await _cartPage.ClearCartAsync();
        await _cartPage.WaitForLoadAsync();

        var isEmpty = await _cartPage.IsEmptyAsync();
        isEmpty.Should().BeTrue();
    }
}
