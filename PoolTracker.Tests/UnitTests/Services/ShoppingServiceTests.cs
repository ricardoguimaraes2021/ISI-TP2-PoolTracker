using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;
using Xunit;

namespace PoolTracker.Tests.UnitTests.Services;

public class ShoppingServiceTests
{
    private readonly Mock<IRepository<ShoppingItem>> _mockRepository;
    private readonly PoolTrackerDbContext _context;
    private readonly ShoppingService _service;

    public ShoppingServiceTests()
    {
        _mockRepository = new Mock<IRepository<ShoppingItem>>();
        
        var options = new DbContextOptionsBuilder<PoolTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new PoolTrackerDbContext(options);
        
        _service = new ShoppingService(_mockRepository.Object, _context);
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldReturnAllItems_WhenNoCategoryFilter()
    {
        // Arrange
        var items = new List<ShoppingItem>
        {
            new ShoppingItem { Id = 1, Name = "Cloro", Category = ShoppingCategory.Qualidade, IsPurchased = false },
            new ShoppingItem { Id = 2, Name = "Café", Category = ShoppingCategory.Bar, IsPurchased = true }
        };
        _context.ShoppingList.AddRange(items);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllItemsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(i => i.Name == "Cloro");
        result.Should().Contain(i => i.Name == "Café");
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldReturnFilteredItems_WhenCategoryProvided()
    {
        // Arrange
        var items = new List<ShoppingItem>
        {
            new ShoppingItem { Id = 1, Name = "Cloro", Category = ShoppingCategory.Qualidade, IsPurchased = false },
            new ShoppingItem { Id = 2, Name = "Café", Category = ShoppingCategory.Bar, IsPurchased = false }
        };
        _context.ShoppingList.AddRange(items);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllItemsAsync(ShoppingCategory.Bar);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(i => i.Name == "Café");
        result.Should().NotContain(i => i.Name == "Cloro");
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldReturnNonPurchasedFirst()
    {
        // Arrange
        var items = new List<ShoppingItem>
        {
            new ShoppingItem { Id = 1, Name = "Item Comprado", Category = ShoppingCategory.Bar, IsPurchased = true, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new ShoppingItem { Id = 2, Name = "Item Pendente", Category = ShoppingCategory.Bar, IsPurchased = false, CreatedAt = DateTime.UtcNow }
        };
        _context.ShoppingList.AddRange(items);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllItemsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Item Pendente"); // Não comprado primeiro
        result.Last().Name.Should().Be("Item Comprado"); // Comprado por último
    }

    [Fact]
    public async Task CreateItemAsync_ShouldCreateItem_WithIsPurchasedFalse()
    {
        // Arrange
        var request = new CreateShoppingItemRequest
        {
            Name = "Novo Item",
            Category = ShoppingCategory.Limpeza
        };

        // Act
        var result = await _service.CreateItemAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Novo Item");
        result.Category.Should().Be("Limpeza");
        result.IsPurchased.Should().BeFalse();
        result.PurchasedAt.Should().BeNull();
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<ShoppingItem>()), Times.Once);
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldUpdateIsPurchased_WhenProvided()
    {
        // Arrange
        var item = new ShoppingItem
        {
            Id = 1,
            Name = "Item Teste",
            Category = ShoppingCategory.Bar,
            IsPurchased = false,
            PurchasedAt = null
        };
        _context.ShoppingList.Add(item);
        await _context.SaveChangesAsync();
        
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<ShoppingItem>())).Returns(Task.CompletedTask);

        var request = new UpdateShoppingItemRequest
        {
            IsPurchased = true
        };

        // Act
        var result = await _service.UpdateItemAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result!.IsPurchased.Should().BeTrue();
        result.PurchasedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task TogglePurchasedAsync_ShouldToggleFromFalseToTrue()
    {
        // Arrange
        var item = new ShoppingItem
        {
            Id = 1,
            Name = "Item Teste",
            Category = ShoppingCategory.Bar,
            IsPurchased = false,
            PurchasedAt = null
        };
        _context.ShoppingList.Add(item);
        await _context.SaveChangesAsync();
        
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<ShoppingItem>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.TogglePurchasedAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.IsPurchased.Should().BeTrue();
        result.PurchasedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task TogglePurchasedAsync_ShouldToggleFromTrueToFalse()
    {
        // Arrange
        var item = new ShoppingItem
        {
            Id = 1,
            Name = "Item Teste",
            Category = ShoppingCategory.Bar,
            IsPurchased = true,
            PurchasedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.ShoppingList.Add(item);
        await _context.SaveChangesAsync();
        
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<ShoppingItem>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.TogglePurchasedAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.IsPurchased.Should().BeFalse();
        result.PurchasedAt.Should().BeNull();
    }

    [Fact]
    public async Task TogglePurchasedAsync_ShouldReturnNull_WhenItemNotExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ShoppingItem?)null);

        // Act
        var result = await _service.TogglePurchasedAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetItemByIdAsync_ShouldReturnItem_WhenExists()
    {
        // Arrange
        var item = new ShoppingItem
        {
            Id = 1,
            Name = "Item Teste",
            Category = ShoppingCategory.Qualidade,
            IsPurchased = true,
            PurchasedAt = DateTime.UtcNow
        };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        // Act
        var result = await _service.GetItemByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Item Teste");
        result.IsPurchased.Should().BeTrue();
        result.PurchasedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task GetItemByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ShoppingItem?)null);

        // Act
        var result = await _service.GetItemByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteItemAsync_ShouldReturnTrue_WhenItemExists()
    {
        // Arrange
        var item = new ShoppingItem
        {
            Id = 1,
            Name = "Item Teste",
            Category = ShoppingCategory.Bar,
            IsPurchased = false
        };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<ShoppingItem>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteItemAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(item), Times.Once);
    }

    [Fact]
    public async Task DeleteItemAsync_ShouldReturnFalse_WhenItemNotExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ShoppingItem?)null);

        // Act
        var result = await _service.DeleteItemAsync(999);

        // Assert
        result.Should().BeFalse();
    }
}

