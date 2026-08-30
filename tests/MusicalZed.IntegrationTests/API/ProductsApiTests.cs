using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MusicalZed.Application.DTOs;
using MusicalZed.IntegrationTests.Helpers;

namespace MusicalZed.IntegrationTests.API;

public class ProductsApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsApiTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_products_ShouldReturn200WithProducts()
    {
        var response = await _client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GET_products_featured_ShouldReturn200WithFeaturedProducts()
    {
        var response = await _client.GetAsync("/api/products/featured");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNull();
        products!.Should().AllSatisfy(p => p.IsFeatured.Should().BeTrue());
    }

    [Fact]
    public async Task GET_products_byId_WhenExists_ShouldReturn200()
    {
        var allResponse = await _client.GetAsync("/api/products");
        var all = await allResponse.Content.ReadFromJsonAsync<List<ProductDto>>();
        var firstId = all!.First().Id;

        var response = await _client.GetAsync($"/api/products/{firstId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product!.Id.Should().Be(firstId);
    }

    [Fact]
    public async Task GET_products_byId_WhenNotExists_ShouldReturn404()
    {
        var response = await _client.GetAsync("/api/products/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_products_withSearch_ShouldReturnFilteredResults()
    {
        var response = await _client.GetAsync("/api/products?search=Guitarra");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNull();
    }

    [Fact]
    public async Task GET_products_withCategoryId_ShouldReturnFilteredResults()
    {
        var categoriesResponse = await _client.GetAsync("/api/categories");
        var categories = await categoriesResponse.Content.ReadFromJsonAsync<List<CategoryDto>>();
        var catId = categories!.First().Id;

        var response = await _client.GetAsync($"/api/products?categoryId={catId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNull();
        products!.Should().AllSatisfy(p => p.CategoryId.Should().Be(catId));
    }
}
