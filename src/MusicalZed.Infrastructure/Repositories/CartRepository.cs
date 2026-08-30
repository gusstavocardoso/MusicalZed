namespace MusicalZed.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using MusicalZed.Domain.Entities;
using MusicalZed.Domain.Interfaces;
using MusicalZed.Infrastructure.Data;

public class CartRepository(MusicalZedDbContext context) : ICartRepository
{
    public async Task<IEnumerable<CartItem>> GetBySessionAsync(string sessionId)
        => await context.CartItems.Include(c => c.Product).Where(c => c.SessionId == sessionId).ToListAsync();

    public async Task<CartItem?> GetItemAsync(string sessionId, int productId)
        => await context.CartItems.Include(c => c.Product).FirstOrDefaultAsync(c => c.SessionId == sessionId && c.ProductId == productId);

    public async Task<CartItem> AddItemAsync(CartItem item)
    {
        context.CartItems.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateItemAsync(CartItem item)
    {
        context.CartItems.Update(item);
        await context.SaveChangesAsync();
    }

    public async Task RemoveItemAsync(int itemId)
    {
        var item = await context.CartItems.FindAsync(itemId);
        if (item is not null)
        {
            context.CartItems.Remove(item);
            await context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(string sessionId)
    {
        var items = context.CartItems.Where(c => c.SessionId == sessionId);
        context.CartItems.RemoveRange(items);
        await context.SaveChangesAsync();
    }
}
