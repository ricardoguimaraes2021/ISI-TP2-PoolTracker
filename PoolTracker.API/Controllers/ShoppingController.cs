using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/shopping")]
[Authorize]
public class ShoppingController : ControllerBase
{
    private readonly IShoppingService _shoppingService;

    public ShoppingController(IShoppingService shoppingService)
    {
        _shoppingService = shoppingService;
    }

    /// <summary>
    /// Listar itens da lista de compras
    /// </summary>
    /// <remarks>
    /// Retorna todos os itens da lista de compras.
    /// Requer autenticação JWT.
    /// Pode filtrar por categoria (Bar, Limpeza, Qualidade).
    /// </remarks>
    /// <param name="category">Categoria para filtrar (opcional)</param>
    /// <returns>Lista de itens da lista de compras</returns>
    /// <response code="200">Lista de itens retornada com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ShoppingItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ShoppingItemDto>>> GetAll([FromQuery] ShoppingCategory? category = null)
    {
        var items = await _shoppingService.GetAllItemsAsync(category);
        return Ok(items);
    }

    /// <summary>
    /// Adicionar item à lista de compras
    /// </summary>
    /// <remarks>
    /// Adiciona um novo item à lista de compras.
    /// Requer autenticação JWT.
    /// O item deve ter um nome e uma categoria (Bar, Limpeza ou Qualidade).
    /// </remarks>
    /// <param name="request">Dados do item a adicionar</param>
    /// <returns>Item criado</returns>
    /// <response code="201">Item adicionado com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost]
    [ProducesResponseType(typeof(ShoppingItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ShoppingItemDto>> Create([FromBody] CreateShoppingItemRequest request)
    {
        var item = await _shoppingService.CreateItemAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    /// <summary>
    /// Obter item da lista de compras por ID
    /// </summary>
    /// <remarks>
    /// Retorna os detalhes de um item específico da lista de compras.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="id">ID do item</param>
    /// <returns>Detalhes do item</returns>
    /// <response code="200">Item encontrado</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Item não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ShoppingItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingItemDto>> GetById(int id)
    {
        var item = await _shoppingService.GetItemByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    /// <summary>
    /// Remover item da lista de compras
    /// </summary>
    /// <remarks>
    /// Remove um item da lista de compras.
    /// Requer autenticação JWT.
    /// Use com cuidado, esta operação não pode ser desfeita.
    /// </remarks>
    /// <param name="id">ID do item a remover</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Item removido com sucesso</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Item não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _shoppingService.DeleteItemAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}

