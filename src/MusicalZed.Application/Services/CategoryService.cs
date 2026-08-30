namespace MusicalZed.Application.Services;

using MusicalZed.Application.DTOs;
using MusicalZed.Application.Interfaces;
using MusicalZed.Domain.Interfaces;

public class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await repository.GetAllAsync();
        return categories.Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.IconClass, c.Products.Count));
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await repository.GetByIdAsync(id);
        return category is null ? null : new CategoryDto(category.Id, category.Name, category.Description, category.IconClass, category.Products.Count);
    }
}
