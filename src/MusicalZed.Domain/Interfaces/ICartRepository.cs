namespace MusicalZed.Domain.Interfaces;
using MusicalZed.Domain.Entities;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>> GetBySessionAsync(string sessionId);
    Task<CartItem?> GetItemAsync(string sessionId, int productId);
    Task<CartItem> AddItemAsync(CartItem item);
    Task UpdateItemAsync(CartItem item);
    Task RemoveItemAsync(int itemId);
    Task ClearCartAsync(string sessionId);
}
