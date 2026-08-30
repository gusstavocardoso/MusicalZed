namespace MusicalZed.Domain.Interfaces;
using MusicalZed.Domain.Entities;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
}
