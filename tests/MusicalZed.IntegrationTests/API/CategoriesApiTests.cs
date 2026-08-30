using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MusicalZed.Application.DTOs;
using MusicalZed.IntegrationTests.Helpers;

namespace MusicalZed.IntegrationTests.API;

public class CategoriesApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriesApiTests(TestWebApplicationFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task GET_categories_ShouldReturn200WithCategories()
    {
        var response = await _client.GetAsync("/api/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        categories.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GET_categories_byId_WhenExists_ShouldReturn200()
    {
        var allResponse = await _client.GetAsync("/api/categories");
        var all = await allResponse.Content.ReadFromJsonAsync<List<CategoryDto>>();
        var id = all!.First().Id;

        var response = await _client.GetAsync($"/api/categories/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cat = await response.Content.ReadFromJsonAsync<CategoryDto>();
        cat!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GET_categories_byId_WhenNotExists_ShouldReturn404()
    {
        var response = await _client.GetAsync("/api/categories/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
