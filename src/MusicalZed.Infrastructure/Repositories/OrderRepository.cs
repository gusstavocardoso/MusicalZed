namespace MusicalZed.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using MusicalZed.Domain.Entities;
using MusicalZed.Domain.Interfaces;
using MusicalZed.Infrastructure.Data;

public class OrderRepository(MusicalZedDbContext context) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(int id)
        => await context.Orders.Include(o => o.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(o => o.Id == id);

    public async Task<IEnumerable<Order>> GetAllAsync()
        => await context.Orders.Include(o => o.Items).OrderByDescending(o => o.CreatedAt).ToListAsync();

    public async Task<Order> AddAsync(Order order)
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return order;
    }

    public async Task UpdateAsync(Order order)
    {
        context.Orders.Update(order);
        await context.SaveChangesAsync();
    }
}
