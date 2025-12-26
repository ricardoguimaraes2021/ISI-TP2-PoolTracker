using PoolTracker.Core.Entities;

namespace PoolTracker.Core.DTOs;

/// <summary>
/// Representa um item da lista de compras
/// </summary>
public class ShoppingItemDto
{
    /// <summary>
    /// ID único do item
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Nome do item
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Categoria do item (Bar, Limpeza, Qualidade)
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Indica se o item já foi comprado
    /// </summary>
    public bool IsPurchased { get; set; }
    
    /// <summary>
    /// Data e hora em que o item foi marcado como comprado (null se não comprado)
    /// </summary>
    public DateTime? PurchasedAt { get; set; }
    
    /// <summary>
    /// Data e hora de criação do item
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Data e hora da última atualização
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request para criar um novo item na lista de compras
/// </summary>
public class CreateShoppingItemRequest
{
    /// <summary>
    /// Nome do item (obrigatório)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Categoria do item: Bar, Limpeza ou Qualidade (obrigatório)
    /// </summary>
    public ShoppingCategory Category { get; set; }
}

/// <summary>
/// Request para atualizar um item da lista de compras
/// </summary>
public class UpdateShoppingItemRequest
{
    /// <summary>
    /// Novo nome do item (opcional)
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Nova categoria do item (opcional)
    /// </summary>
    public ShoppingCategory? Category { get; set; }
    
    /// <summary>
    /// Marcar como comprado/não comprado (opcional). Use o endpoint toggle-purchased para alternar.
    /// </summary>
    public bool? IsPurchased { get; set; }
}

