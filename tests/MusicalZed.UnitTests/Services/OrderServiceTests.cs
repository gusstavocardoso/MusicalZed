using FluentAssertions;
using Moq;
using MusicalZed.Application.DTOs;
using MusicalZed.Application.Services;
using MusicalZed.Domain.Entities;
using MusicalZed.Domain.Interfaces;

namespace MusicalZed.UnitTests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderMock = new();
    private readonly Mock<ICartRepository> _cartMock = new();
    private readonly OrderService _sut;
    private const string Session = "order-test-session";

    public OrderServiceTests() => _sut = new OrderService(_orderMock.Object, _cartMock.Object);

    private static CartItem MakeCartItem(int id, decimal price, int qty) =>
        new()
        {
            Id = id, SessionId = Session, ProductId = id, Quantity = qty, UnitPrice = price,
            Product = new Product { Id = id, Name = $"Produto {id}", Price = price, CategoryId = 1, Category = new Category { Id = 1, Name = "Cat" } }
        };

    private static CreateOrderRequest MakeRequest(string session = Session) =>
        new("João Silva", "joao@test.com", "11999999999", "Rua A, 1", "SP", "SP", "01001-000", "PIX", session, "");

    [Fact]
    public async Task CreateAsync_WhenCartEmpty_ShouldThrowInvalidOperation()
    {
        _cartMock.Setup(r => r.GetBySessionAsync(Session)).ReturnsAsync([]);

        Func<Task> act = () => _sut.CreateAsync(MakeRequest());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*vazio*");
    }

    [Fact]
    public async Task CreateAsync_WhenSubtotalBelowFreeShipping_ShouldChargeShipping()
    {
        var items = new[] { MakeCartItem(1, 200m, 2) }; // total = 400
        _cartMock.Setup(r => r.GetBySessionAsync(Session)).ReturnsAsync(items);
        _orderMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => { o.Id = 1; return o; });

        var order = await _sut.CreateAsync(MakeRequest());

        order.Subtotal.Should().Be(400m);
        order.ShippingCost.Should().Be(29.90m);
        order.Total.Should().Be(429.90m);
    }

    [Fact]
    public async Task CreateAsync_WhenSubtotalAboveFreeShipping_ShouldNotChargeShipping()
    {
        var items = new[] { MakeCartItem(1, 300m, 2) }; // total = 600
        _cartMock.Setup(r => r.GetBySessionAsync(Session)).ReturnsAsync(items);
        _orderMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => { o.Id = 2; return o; });

        var order = await _sut.CreateAsync(MakeRequest());

        order.Subtotal.Should().Be(600m);
        order.ShippingCost.Should().Be(0m);
        order.Total.Should().Be(600m);
    }

    [Fact]
    public async Task CreateAsync_ShouldClearCartAfterOrder()
    {
        var items = new[] { MakeCartItem(1, 600m, 1) };
        _cartMock.Setup(r => r.GetBySessionAsync(Session)).ReturnsAsync(items);
        _orderMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => { o.Id = 3; return o; });

        await _sut.CreateAsync(MakeRequest());

        _cartMock.Verify(r => r.ClearCartAsync(Session), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldMapCustomerDataCorrectly()
    {
        var items = new[] { MakeCartItem(1, 600m, 1) };
        _cartMock.Setup(r => r.GetBySessionAsync(Session)).ReturnsAsync(items);
        _orderMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => { o.Id = 4; return o; });

        var order = await _sut.CreateAsync(MakeRequest());

        order.CustomerName.Should().Be("João Silva");
        order.Email.Should().Be("joao@test.com");
        order.PaymentMethod.Should().Be("PIX");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ShouldReturnNull()
    {
        _orderMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Order?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldSetShippingToExactly500Threshold()
    {
        var items = new[] { MakeCartItem(1, 500m, 1) }; // exatamente 500 = grátis
        _cartMock.Setup(r => r.GetBySessionAsync(Session)).ReturnsAsync(items);
        _orderMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => { o.Id = 5; return o; });

        var order = await _sut.CreateAsync(MakeRequest());

        order.ShippingCost.Should().Be(0m);
    }
}
