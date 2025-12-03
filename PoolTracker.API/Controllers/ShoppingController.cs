using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    [HttpGet]
    public async Task<ActionResult<List<ShoppingItemDto>>> GetAll([FromQuery] ShoppingCategory? category = null)
    {
        var items = await _shoppingService.GetAllItemsAsync(category);
        return Ok(items);
    }

    /// <summary>
    /// Adicionar item à lista de compras
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ShoppingItemDto>> Create([FromBody] CreateShoppingItemRequest request)
    {
        var item = await _shoppingService.CreateItemAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    /// <summary>
    /// Obter item por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ShoppingItemDto>> GetById(int id)
    {
        var item = await _shoppingService.GetItemByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    /// <summary>
    /// Remover item da lista de compras
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _shoppingService.DeleteItemAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}

