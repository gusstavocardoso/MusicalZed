namespace MusicalZed.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using MusicalZed.Domain.Entities;
using MusicalZed.Domain.Interfaces;
using MusicalZed.Infrastructure.Data;

public class CategoryRepository(MusicalZedDbContext context) : ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetAllAsync()
        => await context.Categories.Include(c => c.Products.Where(p => p.IsActive)).OrderBy(c => c.Name).ToListAsync();

    public async Task<Category?> GetByIdAsync(int id)
        => await context.Categories.Include(c => c.Products.Where(p => p.IsActive)).FirstOrDefaultAsync(c => c.Id == id);
}
