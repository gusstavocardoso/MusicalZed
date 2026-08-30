namespace MusicalZed.Application.Interfaces;
using MusicalZed.Application.DTOs;

public interface ICartService
{
    Task<CartDto> GetCartAsync(string sessionId);
    Task<CartDto> AddItemAsync(string sessionId, AddToCartRequest request);
    Task<CartDto> UpdateItemAsync(string sessionId, int productId, UpdateCartItemRequest request);
    Task<CartDto> RemoveItemAsync(string sessionId, int productId);
    Task ClearCartAsync(string sessionId);
}
