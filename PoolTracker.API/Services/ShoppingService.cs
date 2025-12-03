using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;

namespace PoolTracker.API.Services;

public class ShoppingService : IShoppingService
{
    private readonly IRepository<ShoppingItem> _repository;
    private readonly PoolTrackerDbContext _context;

    public ShoppingService(IRepository<ShoppingItem> repository, PoolTrackerDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<List<ShoppingItemDto>> GetAllItemsAsync(ShoppingCategory? category = null)
    {
        var query = _context.ShoppingList.AsQueryable();

        if (category.HasValue)
        {
            query = query.Where(item => item.Category == category.Value);
        }

        var items = await query
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync();

        return items.Select(item => new ShoppingItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Category = item.Category.ToString(),
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        }).ToList();
    }

    public async Task<ShoppingItemDto?> GetItemByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return null;

        return new ShoppingItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Category = item.Category.ToString(),
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    public async Task<ShoppingItemDto> CreateItemAsync(CreateShoppingItemRequest request)
    {
        var item = new ShoppingItem
        {
            Name = request.Name,
            Category = request.Category
        };

        await _repository.AddAsync(item);

        return new ShoppingItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Category = item.Category.ToString(),
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return false;

        await _repository.DeleteAsync(item);
        return true;
    }
}

