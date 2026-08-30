namespace MusicalZed.Application.DTOs;

public record ProductDto(
    int Id,
    string Name,
    string Description,
    string ShortDescription,
    decimal Price,
    decimal? OriginalPrice,
    string ImageUrl,
    string Brand,
    string SKU,
    int StockQuantity,
    bool IsFeatured,
    bool IsActive,
    double Rating,
    int ReviewCount,
    int CategoryId,
    string CategoryName,
    DateTime CreatedAt
);
