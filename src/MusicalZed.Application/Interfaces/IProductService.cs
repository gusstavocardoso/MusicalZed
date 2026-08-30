namespace MusicalZed.Application.Interfaces;
using MusicalZed.Application.DTOs;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<IEnumerable<ProductDto>> GetFeaturedAsync();
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDto>> SearchAsync(string query);
    Task<ProductDto?> GetByIdAsync(int id);
}
