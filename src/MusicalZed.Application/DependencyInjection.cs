namespace MusicalZed.Application;

using Microsoft.Extensions.DependencyInjection;
using MusicalZed.Application.Interfaces;
using MusicalZed.Application.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}
