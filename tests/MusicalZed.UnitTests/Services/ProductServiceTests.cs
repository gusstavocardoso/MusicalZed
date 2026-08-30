using FluentAssertions;
using Moq;
using MusicalZed.Application.Services;
using MusicalZed.Domain.Entities;
using MusicalZed.Domain.Interfaces;

namespace MusicalZed.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repoMock = new();
    private readonly ProductService _sut;

    public ProductServiceTests() => _sut = new ProductService(_repoMock.Object);

    private static Product MakeProduct(int id = 1, string name = "Guitarra Test", decimal price = 100m, bool featured = false) =>
        new()
        {
            Id = id, Name = name, Description = "Desc", ShortDescription = "Short",
            Price = price, Brand = "BrandX", SKU = $"SKU-{id}", StockQuantity = 10,
            IsFeatured = featured, IsActive = true, Rating = 4.5, ReviewCount = 10,
            CategoryId = 1, Category = new Category { Id = 1, Name = "Cat" }
        };

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        var products = new[] { MakeProduct(1), MakeProduct(2) };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Guitarra Test");
    }

    [Fact]
    public async Task GetFeaturedAsync_ShouldReturnOnlyFeaturedProducts()
    {
        var products = new[] { MakeProduct(1, featured: true), MakeProduct(2, featured: true) };
        _repoMock.Setup(r => r.GetFeaturedAsync()).ReturnsAsync(products);

        var result = await _sut.GetFeaturedAsync();

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.IsFeatured.Should().BeTrue());
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnProduct()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeProduct(1));

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Guitarra Test");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        var result = await _sut.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCategoryAsync_ShouldReturnFilteredProducts()
    {
        var products = new[] { MakeProduct(1), MakeProduct(2) };
        _repoMock.Setup(r => r.GetByCategoryAsync(1)).ReturnsAsync(products);

        var result = await _sut.GetByCategoryAsync(1);

        result.Should().HaveCount(2);
        _repoMock.Verify(r => r.GetByCategoryAsync(1), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingProducts()
    {
        var products = new[] { MakeProduct(1, "Guitarra Fender") };
        _repoMock.Setup(r => r.SearchAsync("Fender")).ReturnsAsync(products);

        var result = await _sut.SearchAsync("Fender");

        result.Should().HaveCount(1);
        result.First().Name.Should().Contain("Fender");
    }

    [Fact]
    public async Task GetAllAsync_ShouldMapAllDtoFields()
    {
        var product = MakeProduct(5, "Les Paul", 18999m);
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync([product]);

        var result = (await _sut.GetAllAsync()).Single();

        result.Id.Should().Be(5);
        result.Name.Should().Be("Les Paul");
        result.Price.Should().Be(18999m);
        result.CategoryName.Should().Be("Cat");
    }
}
