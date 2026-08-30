namespace MusicalZed.Application.Interfaces;
using MusicalZed.Application.DTOs;

public interface IOrderService
{
    Task<OrderDto?> GetByIdAsync(int id);
    Task<OrderDto> CreateAsync(CreateOrderRequest request);
}
