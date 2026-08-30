namespace MusicalZed.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using MusicalZed.Domain.Entities;
using MusicalZed.Domain.Interfaces;
using MusicalZed.Infrastructure.Data;

public class ProductRepository(MusicalZedDbContext context) : IProductRepository
{
    public async Task<IEnumerable<Product>> GetAllAsync()
        => await context.Products.Include(p => p.Category).Where(p => p.IsActive).OrderBy(p => p.Name).ToListAsync();

    public async Task<IEnumerable<Product>> GetFeaturedAsync()
        => await context.Products.Include(p => p.Category).Where(p => p.IsActive && p.IsFeatured).Take(8).ToListAsync();

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        => await context.Products.Include(p => p.Category).Where(p => p.IsActive && p.CategoryId == categoryId).OrderBy(p => p.Name).ToListAsync();

    public async Task<IEnumerable<Product>> SearchAsync(string query)
    {
        var lower = query.ToLower();
        return await context.Products.Include(p => p.Category)
            .Where(p => p.IsActive && (p.Name.ToLower().Contains(lower) || p.Brand.ToLower().Contains(lower) || p.Description.ToLower().Contains(lower)))
            .OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
        => await context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Product> AddAsync(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product is not null)
        {
            product.IsActive = false;
            await context.SaveChangesAsync();
        }
    }
}
