namespace PoolTracker.Core.Entities;

public class ShoppingItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ShoppingCategory Category { get; set; }
    public bool IsPurchased { get; set; } = false;
    public DateTime? PurchasedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum ShoppingCategory
{
    Bar,
    Limpeza,
    Qualidade
}

