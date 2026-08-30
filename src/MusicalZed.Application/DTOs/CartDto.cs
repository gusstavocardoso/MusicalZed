namespace MusicalZed.Application.DTOs;

public record CartItemDto(
    int Id,
    int ProductId,
    string ProductName,
    string ProductImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal
);

public record CartDto(string SessionId, IEnumerable<CartItemDto> Items, decimal Total, int ItemCount);

public record AddToCartRequest(int ProductId, int Quantity);
public record UpdateCartItemRequest(int Quantity);
