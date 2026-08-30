namespace MusicalZed.Application.Services;

using MusicalZed.Application.DTOs;
using MusicalZed.Application.Interfaces;
using MusicalZed.Domain.Entities;
using MusicalZed.Domain.Interfaces;

public class OrderService(
    IOrderRepository orderRepository,
    ICartRepository cartRepository) : IOrderService
{
    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await orderRepository.GetByIdAsync(id);
        return order is null ? null : MapToDto(order);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request)
    {
        var cartItems = (await cartRepository.GetBySessionAsync(request.SessionId)).ToList();
        if (!cartItems.Any())
            throw new InvalidOperationException("Carrinho está vazio.");

        var subtotal = cartItems.Sum(i => i.UnitPrice * i.Quantity);
        var shipping = subtotal >= 500 ? 0m : 29.90m;
        var total = subtotal + shipping;

        var order = new Order
        {
            CustomerName = request.CustomerName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            PaymentMethod = request.PaymentMethod,
            SessionId = request.SessionId,
            Notes = request.Notes,
            Subtotal = subtotal,
            ShippingCost = shipping,
            Total = total,
            Items = cartItems.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                ProductImageUrl = i.Product?.ImageUrl ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Total = i.UnitPrice * i.Quantity
            }).ToList()
        };

        var created = await orderRepository.AddAsync(order);
        await cartRepository.ClearCartAsync(request.SessionId);
        return MapToDto(created);
    }

    private static OrderDto MapToDto(Order o) => new(
        o.Id, o.CustomerName, o.Email, o.Phone,
        o.Address, o.City, o.State, o.ZipCode,
        o.PaymentMethod, o.Status.ToString(),
        o.Subtotal, o.ShippingCost, o.Total, o.CreatedAt,
        o.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.ProductImageUrl, i.Quantity, i.UnitPrice, i.Total))
    );
}
