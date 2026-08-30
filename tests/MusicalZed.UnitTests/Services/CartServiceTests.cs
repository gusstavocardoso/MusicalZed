using FluentAssertions;
using Moq;
using MusicalZed.Application.DTOs;
using MusicalZed.Application.Services;
using MusicalZed.Domain.Entities;
using MusicalZed.Domain.Interfaces;

namespace MusicalZed.UnitTests.Services;

public class CartServiceTests
{
    private readonly Mock<ICartRepository> _cartMock = new();
    private readonly Mock<IProductRepository> _productMock = new();
    private readonly CartService _sut;
    private const string Session = "test-session-id";

    public CartServiceTests() => _sut = new CartService(_cartMock.Object, _productMock.Object);

    private static Product MakeProduct(int id = 1, decimal price = 299m) =>
        new() { Id = id, Name = $"Produto {id}", Price = price, StockQuantity = 5, CategoryId = 1, Category = new Category { Id = 1, Name = "Cat" } };

    private static CartItem MakeCartItem(int productId = 1, int qty = 2, decimal price = 299m) =>
        new() { Id = productId, SessionId = Session, ProductId = productId, Quantity = qty, UnitPrice = price, Product = MakeProduct(productId, price) };

    [Fact]
    public async Task GetCartAsync_WhenEmpty_ShouldReturnEmptyCart()
    {
        _cartMock.Setup(r => r.GetBySessionAsync(Session)).ReturnsAsync([]);

        var cart = await _sut.GetCartAsync(Session);

        cart.SessionId.Should().Be(Session);
        cart.Items.Should().BeEmpty();
        cart.Total.Should().Be(0);
        cart.ItemCount.Should().Be(0);
    }

    [Fact]
    public async Task GetCartAsync_WithItems_ShouldCalculateTotal()
    {
        var items = new[] { MakeCartItem(1, 2, 300m), MakeCartItem(2, 1, 100m) };
        _cartMock.Setup(r => r.GetBySessionAsync(Session)).ReturnsAsync(items);

        var cart = await _sut.GetCartAsync(Session);

        cart.Total.Should().Be(700m); // 2*300 + 1*100
        cart.ItemCount.Should().Be(3);
        cart.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddItemAsync_WhenProductNotFound_ShouldThrowKeyNotFoundException()
    {
        _productMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        Func<Task> act = () => _sut.AddItemAsync(Session, new AddToCartRequest(99, 1));

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task AddItemAsync_WhenNewItem_ShouldAddToCart()
    {
        var product = MakeProduct(1, 500m);
        _productMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _cartMock.Setup(r => r.GetItemAsync(Session, 1)).ReturnsAsync((CartItem?)null);
        _cartMock.Setup(r => r.AddItemAsync(It.IsAny<CartItem>()))
            .ReturnsAsync((CartItem ci) => ci);
        _cartMock.Setup(r => r.GetBySessionAsync(Session))
            .ReturnsAsync([MakeCartItem(1, 1, 500m)]);

        var cart = await _sut.AddItemAsync(Session, new AddToCartRequest(1, 1));

        cart.Items.Should().HaveCount(1);
        _cartMock.Verify(r => r.AddItemAsync(It.Is<CartItem>(ci => ci.ProductId == 1 && ci.Quantity == 1)), Times.Once);
    }

    [Fact]
    public async Task AddItemAsync_WhenItemExists_ShouldIncrementQuantity()
    {
        var product = MakeProduct(1, 300m);
        var existing = MakeCartItem(1, 2, 300m);
        _productMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _cartMock.Setup(r => r.GetItemAsync(Session, 1)).ReturnsAsync(existing);
        _cartMock.Setup(r => r.GetBySessionAsync(Session))
            .ReturnsAsync([MakeCartItem(1, 3, 300m)]);

        var cart = await _sut.AddItemAsync(Session, new AddToCartRequest(1, 1));

        _cartMock.Verify(r => r.UpdateItemAsync(It.Is<CartItem>(ci => ci.Quantity == 3)), Times.Once);
    }

    [Fact]
    public async Task RemoveItemAsync_WhenItemNotFound_ShouldThrowKeyNotFoundException()
    {
        _cartMock.Setup(r => r.GetItemAsync(Session, 99)).ReturnsAsync((CartItem?)null);

        Func<Task> act = () => _sut.RemoveItemAsync(Session, 99);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ClearCartAsync_ShouldCallRepository()
    {
        _cartMock.Setup(r => r.ClearCartAsync(Session)).Returns(Task.CompletedTask);

        await _sut.ClearCartAsync(Session);

        _cartMock.Verify(r => r.ClearCartAsync(Session), Times.Once);
    }
}
