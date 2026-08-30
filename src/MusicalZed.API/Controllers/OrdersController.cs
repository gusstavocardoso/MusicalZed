namespace MusicalZed.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using MusicalZed.Application.DTOs;
using MusicalZed.Application.Interfaces;

/// <summary>
/// Gerenciamento de pedidos da loja Musical Zed.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Pedidos")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    /// <summary>
    /// Obtém um pedido pelo ID.
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <returns>Dados completos do pedido com itens</returns>
    /// <response code="200">Pedido encontrado</response>
    /// <response code="404">Pedido não encontrado</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await orderService.GetByIdAsync(id);
        return order is null
            ? NotFound(new { message = $"Pedido {id} não encontrado." })
            : Ok(order);
    }

    /// <summary>
    /// Cria um novo pedido a partir do carrinho da sessão.
    /// Após a criação, o carrinho da sessão é limpo automaticamente.
    /// 
    /// Regras de frete:
    /// - Subtotal >= R$ 500,00 → frete grátis
    /// - Subtotal &lt; R$ 500,00 → frete de R$ 29,90
    /// </summary>
    /// <param name="request">Dados do cliente e sessão do carrinho</param>
    /// <returns>Pedido criado com ID e total calculado</returns>
    /// <response code="201">Pedido criado com sucesso</response>
    /// <response code="400">Carrinho vazio ou dados inválidos</response>
    /// <response code="404">Produto do carrinho não encontrado</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        try
        {
            var order = await orderService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}
