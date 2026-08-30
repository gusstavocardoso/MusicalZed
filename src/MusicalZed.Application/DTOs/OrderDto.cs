namespace MusicalZed.Application.DTOs;

public record OrderItemDto(
    int ProductId,
    string ProductName,
    string ProductImageUrl,
    int Quantity,
    decimal UnitPrice,
    decimal Total
);

public record OrderDto(
    int Id,
    string CustomerName,
    string Email,
    string Phone,
    string Address,
    string City,
    string State,
    string ZipCode,
    string PaymentMethod,
    string Status,
    decimal Subtotal,
    decimal ShippingCost,
    decimal Total,
    DateTime CreatedAt,
    IEnumerable<OrderItemDto> Items
);

public record CreateOrderRequest(
    string CustomerName,
    string Email,
    string Phone,
    string Address,
    string City,
    string State,
    string ZipCode,
    string PaymentMethod,
    string SessionId,
    string Notes
);
