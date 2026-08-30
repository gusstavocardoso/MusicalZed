using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MusicalZed.Application.DTOs;
using MusicalZed.IntegrationTests.Helpers;

namespace MusicalZed.IntegrationTests.API;

public class OrderApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrderApiTests(TestWebApplicationFactory factory)
        => _client = factory.CreateClient();

    private async Task<string> CreateCartWithItem()
    {
        var sessionId = Guid.NewGuid().ToString();
        var products = await _client.GetFromJsonAsync<List<ProductDto>>("/api/products");
        await _client.PostAsJsonAsync($"/api/carts/{sessionId}/items",
            new AddToCartRequest(products!.First().Id, 1));
        return sessionId;
    }

    [Fact]
    public async Task POST_orders_WithValidCart_ShouldReturn201()
    {
        var sessionId = await CreateCartWithItem();
        var request = new CreateOrderRequest(
            "Maria Teste", "maria@test.com", "11988887777",
            "Av. Paulista, 1000", "São Paulo", "SP", "01310-100",
            "PIX", sessionId, "");

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.CustomerName.Should().Be("Maria Teste");
        order.Total.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task POST_orders_WithEmptyCart_ShouldReturn400()
    {
        var emptySession = Guid.NewGuid().ToString();
        var request = new CreateOrderRequest(
            "Test", "t@t.com", "11000000000",
            "Rua X", "SP", "SP", "00000-000", "PIX", emptySession, "");

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_orders_byId_WhenExists_ShouldReturn200()
    {
        var sessionId = await CreateCartWithItem();
        var createResp = await _client.PostAsJsonAsync("/api/orders",
            new CreateOrderRequest("Ana", "ana@t.com", "11111111111",
                "Rua B", "RJ", "RJ", "20000-000", "Boleto", sessionId, ""));
        var created = await createResp.Content.ReadFromJsonAsync<OrderDto>();

        var response = await _client.GetAsync($"/api/orders/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        order!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GET_orders_byId_WhenNotExists_ShouldReturn404()
    {
        var response = await _client.GetAsync("/api/orders/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
