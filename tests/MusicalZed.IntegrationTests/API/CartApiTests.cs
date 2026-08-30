using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MusicalZed.Application.DTOs;
using MusicalZed.IntegrationTests.Helpers;

namespace MusicalZed.IntegrationTests.API;

public class CartApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _sessionId = Guid.NewGuid().ToString();

    public CartApiTests(TestWebApplicationFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task GET_cart_NewSession_ShouldReturnEmptyCart()
    {
        var response = await _client.GetAsync($"/api/carts/{_sessionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cart = await response.Content.ReadFromJsonAsync<CartDto>();
        cart!.Items.Should().BeEmpty();
        cart.Total.Should().Be(0);
    }

    [Fact]
    public async Task POST_cart_AddItem_ShouldReturn200WithUpdatedCart()
    {
        var products = await _client.GetFromJsonAsync<List<ProductDto>>("/api/products");
        var productId = products!.First().Id;

        var response = await _client.PostAsJsonAsync(
            $"/api/carts/{_sessionId}/items",
            new AddToCartRequest(productId, 2));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cart = await response.Content.ReadFromJsonAsync<CartDto>();
        cart!.Items.Should().ContainSingle();
        cart.Items.First().Quantity.Should().Be(2);
    }

    [Fact]
    public async Task DELETE_cart_ShouldClearCart()
    {
        var products = await _client.GetFromJsonAsync<List<ProductDto>>("/api/products");
        await _client.PostAsJsonAsync($"/api/carts/{_sessionId}/items",
            new AddToCartRequest(products!.First().Id, 1));

        var response = await _client.DeleteAsync($"/api/carts/{_sessionId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var cart = await _client.GetFromJsonAsync<CartDto>($"/api/carts/{_sessionId}");
        cart!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task POST_cart_WithInvalidProduct_ShouldReturn404()
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/carts/{_sessionId}/items",
            new AddToCartRequest(99999, 1));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
