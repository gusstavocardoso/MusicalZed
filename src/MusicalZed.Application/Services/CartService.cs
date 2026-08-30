namespace MusicalZed.Application.Services;

using MusicalZed.Application.DTOs;
using MusicalZed.Application.Interfaces;
using MusicalZed.Domain.Entities;
using MusicalZed.Domain.Interfaces;

public class CartService(ICartRepository cartRepository, IProductRepository productRepository) : ICartService
{
    public async Task<CartDto> GetCartAsync(string sessionId)
    {
        var items = await cartRepository.GetBySessionAsync(sessionId);
        return BuildCartDto(sessionId, items);
    }

    public async Task<CartDto> AddItemAsync(string sessionId, AddToCartRequest request)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId)
            ?? throw new KeyNotFoundException($"Produto {request.ProductId} não encontrado.");

        var existing = await cartRepository.GetItemAsync(sessionId, request.ProductId);
        if (existing is not null)
        {
            existing.Quantity += request.Quantity;
            await cartRepository.UpdateItemAsync(existing);
        }
        else
        {
            await cartRepository.AddItemAsync(new CartItem
            {
                SessionId = sessionId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                UnitPrice = product.Price
            });
        }

        return await GetCartAsync(sessionId);
    }

    public async Task<CartDto> UpdateItemAsync(string sessionId, int productId, UpdateCartItemRequest request)
    {
        var item = await cartRepository.GetItemAsync(sessionId, productId)
            ?? throw new KeyNotFoundException("Item não encontrado no carrinho.");

        if (request.Quantity <= 0)
            await cartRepository.RemoveItemAsync(item.Id);
        else
        {
            item.Quantity = request.Quantity;
            await cartRepository.UpdateItemAsync(item);
        }

        return await GetCartAsync(sessionId);
    }

    public async Task<CartDto> RemoveItemAsync(string sessionId, int productId)
    {
        var item = await cartRepository.GetItemAsync(sessionId, productId)
            ?? throw new KeyNotFoundException("Item não encontrado no carrinho.");
        await cartRepository.RemoveItemAsync(item.Id);
        return await GetCartAsync(sessionId);
    }

    public async Task ClearCartAsync(string sessionId)
        => await cartRepository.ClearCartAsync(sessionId);

    private static CartDto BuildCartDto(string sessionId, IEnumerable<CartItem> items)
    {
        var itemList = items.ToList();
        var dtos = itemList.Select(i => new CartItemDto(
            i.Id, i.ProductId,
            i.Product?.Name ?? string.Empty,
            i.Product?.ImageUrl ?? string.Empty,
            i.UnitPrice, i.Quantity, i.UnitPrice * i.Quantity
        )).ToList();
        return new CartDto(sessionId, dtos, dtos.Sum(i => i.Subtotal), dtos.Sum(i => i.Quantity));
    }
}
