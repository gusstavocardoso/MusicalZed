namespace MusicalZed.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using MusicalZed.Application.DTOs;
using MusicalZed.Application.Interfaces;

/// <summary>
/// Gerenciamento de categorias de instrumentos musicais.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Categorias")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    /// <summary>
    /// Lista todas as categorias disponíveis.
    /// </summary>
    /// <returns>Lista de categorias com contagem de produtos</returns>
    /// <response code="200">Lista de categorias retornada com sucesso</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
        => Ok(await categoryService.GetAllAsync());

    /// <summary>
    /// Obtém uma categoria pelo ID.
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <returns>Dados da categoria</returns>
    /// <response code="200">Categoria encontrada</response>
    /// <response code="404">Categoria não encontrada</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await categoryService.GetByIdAsync(id);
        return category is null
            ? NotFound(new { message = $"Categoria {id} não encontrada." })
            : Ok(category);
    }
}
