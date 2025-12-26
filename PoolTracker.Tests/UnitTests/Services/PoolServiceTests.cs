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

public class PoolServiceTests
{
    private readonly Mock<IRepository<PoolStatus>> _mockRepository;
    private readonly Mock<IVisitService> _mockVisitService;
    private readonly PoolTrackerDbContext _context;
    private readonly PoolService _service;

    public PoolServiceTests()
    {
        _mockRepository = new Mock<IRepository<PoolStatus>>();
        _mockVisitService = new Mock<IVisitService>();
        
        var options = new DbContextOptionsBuilder<PoolTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new PoolTrackerDbContext(options);
        
        _service = new PoolService(
            _mockRepository.Object,
            _context,
            _mockVisitService.Object);
    }

    [Fact]
    public async Task GetStatusAsync_ShouldReturnPoolStatus_WhenStatusExists()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 10,
            MaxCapacity = 120,
            IsOpen = true,
            LocationName = "Test Pool",
            Address = "Test Address",
            Phone = "123456789"
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetStatusAsync();

        // Assert
        result.Should().NotBeNull();
        result.CurrentCount.Should().Be(10);
        result.MaxCapacity.Should().Be(120);
        result.IsOpen.Should().BeTrue();
        result.LocationName.Should().Be("Test Pool");
    }

    [Fact]
    public async Task GetStatusAsync_ShouldCreateDefaultStatus_WhenStatusDoesNotExist()
    {
        // Arrange - No status in database

        // Act
        var result = await _service.GetStatusAsync();

        // Assert
        result.Should().NotBeNull();
        result.CurrentCount.Should().Be(0);
        result.MaxCapacity.Should().Be(120);
        result.IsOpen.Should().BeTrue();
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<PoolStatus>()), Times.Once);
    }

    [Fact]
    public async Task EnterAsync_ShouldIncrementCount_WhenPoolIsOpenAndNotFull()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 5,
            MaxCapacity = 120,
            IsOpen = true
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.EnterAsync();

        // Assert
        result.CurrentCount.Should().Be(6);
        _mockVisitService.Verify(v => v.IncrementDailyVisitorsAsync(), Times.Once);
    }

    [Fact]
    public async Task EnterAsync_ShouldNotIncrementCount_WhenPoolIsClosed()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 5,
            MaxCapacity = 120,
            IsOpen = false
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.EnterAsync();

        // Assert
        result.CurrentCount.Should().Be(5);
        _mockVisitService.Verify(v => v.IncrementDailyVisitorsAsync(), Times.Never);
    }

    [Fact]
    public async Task EnterAsync_ShouldNotIncrementCount_WhenPoolIsFull()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 120,
            MaxCapacity = 120,
            IsOpen = true
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.EnterAsync();

        // Assert
        result.CurrentCount.Should().Be(120);
        _mockVisitService.Verify(v => v.IncrementDailyVisitorsAsync(), Times.Never);
    }

    [Fact]
    public async Task ExitAsync_ShouldDecrementCount_WhenCountIsGreaterThanZero()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 10,
            MaxCapacity = 120,
            IsOpen = true
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.ExitAsync();

        // Assert
        result.CurrentCount.Should().Be(9);
    }

    [Fact]
    public async Task ExitAsync_ShouldNotDecrementCount_WhenCountIsZero()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 0,
            MaxCapacity = 120,
            IsOpen = true
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.ExitAsync();

        // Assert
        result.CurrentCount.Should().Be(0);
    }

    [Fact]
    public async Task SetCountAsync_ShouldSetCount_WhenValueIsValid()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 10,
            MaxCapacity = 120,
            IsOpen = true
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SetCountAsync(50);

        // Assert
        result.CurrentCount.Should().Be(50);
    }

    [Fact]
    public async Task SetCountAsync_ShouldClampToMaxCapacity_WhenValueExceedsMax()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 10,
            MaxCapacity = 120,
            IsOpen = true
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SetCountAsync(200);

        // Assert
        result.CurrentCount.Should().Be(120);
    }

    [Fact]
    public async Task SetCountAsync_ShouldClampToZero_WhenValueIsNegative()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 10,
            MaxCapacity = 120,
            IsOpen = true
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SetCountAsync(-10);

        // Assert
        result.CurrentCount.Should().Be(0);
    }

    [Fact]
    public async Task SetCapacityAsync_ShouldSetCapacity_WhenValueIsValid()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 10,
            MaxCapacity = 120,
            IsOpen = true
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SetCapacityAsync(150);

        // Assert
        result.MaxCapacity.Should().Be(150);
    }

    [Fact]
    public async Task SetCapacityAsync_ShouldAdjustCurrentCount_WhenNewCapacityIsLessThanCurrent()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 100,
            MaxCapacity = 120,
            IsOpen = true
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SetCapacityAsync(80);

        // Assert
        result.MaxCapacity.Should().Be(80);
        result.CurrentCount.Should().Be(80);
    }

    [Fact]
    public async Task SetCapacityAsync_ShouldSetMinimumToOne_WhenValueIsZero()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 10,
            MaxCapacity = 120,
            IsOpen = true
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SetCapacityAsync(0);

        // Assert
        result.MaxCapacity.Should().Be(1);
    }

    [Fact]
    public async Task SetOpenStatusAsync_ShouldSetIsOpen_WhenOpening()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 0,
            MaxCapacity = 120,
            IsOpen = false
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SetOpenStatusAsync(true);

        // Assert
        result.IsOpen.Should().BeTrue();
    }

    [Fact]
    public async Task SetOpenStatusAsync_ShouldResetCount_WhenClosing()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 50,
            MaxCapacity = 120,
            IsOpen = true
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SetOpenStatusAsync(false);

        // Assert
        result.IsOpen.Should().BeFalse();
        result.CurrentCount.Should().Be(0);
    }

    [Fact]
    public async Task ResetAsync_ShouldResetCountAndClosePool()
    {
        // Arrange
        var status = new PoolStatus
        {
            Id = 1,
            CurrentCount = 50,
            MaxCapacity = 120,
            IsOpen = true
        };
        _context.PoolStatus.Add(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.ResetAsync();

        // Assert
        result.CurrentCount.Should().Be(0);
        result.IsOpen.Should().BeFalse();
    }
}

