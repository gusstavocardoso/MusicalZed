namespace MusicalZed.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using MusicalZed.Application.DTOs;
using MusicalZed.Application.Interfaces;

/// <summary>
/// Gerenciamento do carrinho de compras por sessão.
/// O carrinho é identificado por um <c>sessionId</c> (UUID único por usuário/dispositivo).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Carrinho")]
public class CartsController(ICartService cartService) : ControllerBase
{
    /// <summary>
    /// Obtém o carrinho de uma sessão.
    /// </summary>
    /// <param name="sessionId">UUID da sessão do usuário</param>
    /// <returns>Dados do carrinho com itens e total</returns>
    /// <response code="200">Carrinho retornado (vazio se não houver itens)</response>
    [HttpGet("{sessionId}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCart(string sessionId)
        => Ok(await cartService.GetCartAsync(sessionId));

    /// <summary>
    /// Adiciona um produto ao carrinho. Se já existir, incrementa a quantidade.
    /// </summary>
    /// <param name="sessionId">UUID da sessão do usuário</param>
    /// <param name="request">Produto e quantidade a adicionar</param>
    /// <returns>Carrinho atualizado</returns>
    /// <response code="200">Item adicionado com sucesso</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpPost("{sessionId}/items")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem(string sessionId, [FromBody] AddToCartRequest request)
    {
        try { return Ok(await cartService.AddItemAsync(sessionId, request)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    /// <summary>
    /// Atualiza a quantidade de um item no carrinho. Se quantidade for 0, remove o item.
    /// </summary>
    /// <param name="sessionId">UUID da sessão do usuário</param>
    /// <param name="productId">ID do produto no carrinho</param>
    /// <param name="request">Nova quantidade</param>
    /// <returns>Carrinho atualizado</returns>
    /// <response code="200">Item atualizado com sucesso</response>
    /// <response code="404">Item não encontrado no carrinho</response>
    [HttpPut("{sessionId}/items/{productId:int}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(string sessionId, int productId, [FromBody] UpdateCartItemRequest request)
    {
        try { return Ok(await cartService.UpdateItemAsync(sessionId, productId, request)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    /// <summary>
    /// Remove um produto do carrinho.
    /// </summary>
    /// <param name="sessionId">UUID da sessão do usuário</param>
    /// <param name="productId">ID do produto a remover</param>
    /// <returns>Carrinho atualizado sem o item</returns>
    /// <response code="200">Item removido com sucesso</response>
    /// <response code="404">Item não encontrado</response>
    [HttpDelete("{sessionId}/items/{productId:int}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(string sessionId, int productId)
    {
        try { return Ok(await cartService.RemoveItemAsync(sessionId, productId)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    /// <summary>
    /// Limpa todos os itens do carrinho de uma sessão.
    /// </summary>
    /// <param name="sessionId">UUID da sessão do usuário</param>
    /// <response code="204">Carrinho limpo com sucesso</response>
    [HttpDelete("{sessionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ClearCart(string sessionId)
    {
        await cartService.ClearCartAsync(sessionId);
        return NoContent();
    }
}
