using PoolTracker.Core.Entities;

namespace PoolTracker.Core.DTOs;

public class ShoppingItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateShoppingItemRequest
{
    public string Name { get; set; } = string.Empty;
    public ShoppingCategory Category { get; set; }
}

public class UpdateShoppingItemRequest
{
    public string? Name { get; set; }
    public ShoppingCategory? Category { get; set; }
}

