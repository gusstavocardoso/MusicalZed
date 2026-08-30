namespace MusicalZed.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using MusicalZed.Application.DTOs;
using MusicalZed.Application.Interfaces;

/// <summary>
/// Gerenciamento de produtos da loja Musical Zed.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Produtos")]
public class ProductsController(IProductService productService) : ControllerBase
{
    /// <summary>
    /// Lista todos os produtos ativos.
    /// </summary>
    /// <param name="search">Termo de busca (nome, marca ou descrição)</param>
    /// <param name="categoryId">Filtrar por ID de categoria</param>
    /// <returns>Lista de produtos</returns>
    /// <response code="200">Lista de produtos retornada com sucesso</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId)
    {
        if (!string.IsNullOrWhiteSpace(search))
            return Ok(await productService.SearchAsync(search));
        if (categoryId.HasValue)
            return Ok(await productService.GetByCategoryAsync(categoryId.Value));
        return Ok(await productService.GetAllAsync());
    }

    /// <summary>
    /// Lista os produtos em destaque (featured).
    /// </summary>
    /// <returns>Lista de produtos em destaque</returns>
    /// <response code="200">Lista de produtos em destaque retornada com sucesso</response>
    [HttpGet("featured")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeatured()
        => Ok(await productService.GetFeaturedAsync());

    /// <summary>
    /// Obtém detalhes de um produto pelo ID.
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>Dados completos do produto</returns>
    /// <response code="200">Produto encontrado</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await productService.GetByIdAsync(id);
        return product is null
            ? NotFound(new { message = $"Produto {id} não encontrado." })
            : Ok(product);
    }
}
