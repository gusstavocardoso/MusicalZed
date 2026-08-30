namespace MusicalZed.Application.Interfaces;
using MusicalZed.Application.DTOs;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
}
