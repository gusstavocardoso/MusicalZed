namespace MusicalZed.Application.Services;

using MusicalZed.Application.DTOs;
using MusicalZed.Application.Interfaces;
using MusicalZed.Domain.Interfaces;

public class ProductService(IProductRepository repository) : IProductService
{
    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await repository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetFeaturedAsync()
    {
        var products = await repository.GetFeaturedAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId)
    {
        var products = await repository.GetByCategoryAsync(categoryId);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> SearchAsync(string query)
    {
        var products = await repository.SearchAsync(query);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await repository.GetByIdAsync(id);
        return product is null ? null : MapToDto(product);
    }

    private static ProductDto MapToDto(Domain.Entities.Product p) => new(
        p.Id, p.Name, p.Description, p.ShortDescription, p.Price, p.OriginalPrice,
        p.ImageUrl, p.Brand, p.SKU, p.StockQuantity, p.IsFeatured, p.IsActive,
        p.Rating, p.ReviewCount, p.CategoryId, p.Category?.Name ?? string.Empty, p.CreatedAt
    );
}
