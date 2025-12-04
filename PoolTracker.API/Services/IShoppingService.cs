using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;

namespace PoolTracker.API.Services;

public interface IShoppingService
{
    Task<List<ShoppingItemDto>> GetAllItemsAsync(ShoppingCategory? category = null);
    Task<ShoppingItemDto?> GetItemByIdAsync(int id);
    Task<ShoppingItemDto> CreateItemAsync(CreateShoppingItemRequest request);
    Task<bool> DeleteItemAsync(int id);
}

