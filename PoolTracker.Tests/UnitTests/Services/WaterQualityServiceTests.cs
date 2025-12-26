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

public class WaterQualityServiceTests
{
    private readonly Mock<IRepository<WaterQuality>> _mockRepository;
    private readonly PoolTrackerDbContext _context;
    private readonly WaterQualityService _service;

    public WaterQualityServiceTests()
    {
        _mockRepository = new Mock<IRepository<WaterQuality>>();
        
        var options = new DbContextOptionsBuilder<PoolTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new PoolTrackerDbContext(options);
        
        _service = new WaterQualityService(_mockRepository.Object, _context);
    }

    [Fact]
    public async Task RecordMeasurementAsync_ShouldCreateMeasurement()
    {
        // Arrange
        var request = new RecordMeasurementRequest
        {
            PoolType = PoolType.Criancas,
            PhLevel = 7.2m,
            Temperature = 26.5m,
            Notes = "Test measurement"
        };

        // Act
        var result = await _service.RecordMeasurementAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.PhLevel.Should().Be(7.2m);
        result.Temperature.Should().Be(26.5m);
        result.PoolType.Should().Be("Criancas");
        result.Notes.Should().Be("Test measurement");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<WaterQuality>()), Times.Once);
    }

    [Fact]
    public async Task GetLatestMeasurementAsync_ShouldReturnLatest_WhenExists()
    {
        // Arrange
        var measurements = new List<WaterQuality>
        {
            new WaterQuality
            {
                Id = 1,
                PoolType = PoolType.Criancas,
                PhLevel = 7.0m,
                Temperature = 25.0m,
                MeasuredAt = DateTime.UtcNow.AddHours(-2)
            },
            new WaterQuality
            {
                Id = 2,
                PoolType = PoolType.Criancas,
                PhLevel = 7.2m,
                Temperature = 26.0m,
                MeasuredAt = DateTime.UtcNow.AddHours(-1)
            }
        };
        _context.WaterQuality.AddRange(measurements);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetLatestMeasurementAsync(PoolType.Criancas);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(2);
        result.PhLevel.Should().Be(7.2m);
    }

    [Fact]
    public async Task GetLatestMeasurementAsync_ShouldReturnNull_WhenNoMeasurements()
    {
        // Act
        var result = await _service.GetLatestMeasurementAsync(PoolType.Criancas);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentMeasurementsAsync_ShouldReturnBothPools_WhenBothExist()
    {
        // Arrange
        var measurements = new List<WaterQuality>
        {
            new WaterQuality
            {
                Id = 1,
                PoolType = PoolType.Criancas,
                PhLevel = 7.2m,
                Temperature = 26.0m,
                MeasuredAt = DateTime.UtcNow
            },
            new WaterQuality
            {
                Id = 2,
                PoolType = PoolType.Adultos,
                PhLevel = 7.3m,
                Temperature = 27.0m,
                MeasuredAt = DateTime.UtcNow
            }
        };
        _context.WaterQuality.AddRange(measurements);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetCurrentMeasurementsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Criancas.Should().NotBeNull();
        result.Adultos.Should().NotBeNull();
        result.Criancas!.PhLevel.Should().Be(7.2m);
        result.Adultos!.PhLevel.Should().Be(7.3m);
    }
}

