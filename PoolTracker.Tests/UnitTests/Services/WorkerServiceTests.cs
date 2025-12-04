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

public class WorkerServiceTests
{
    private readonly Mock<IRepository<Worker>> _mockRepository;
    private readonly Mock<IRepository<ActiveWorker>> _mockActiveWorkerRepository;
    private readonly PoolTrackerDbContext _context;
    private readonly WorkerService _service;

    public WorkerServiceTests()
    {
        _mockRepository = new Mock<IRepository<Worker>>();
        _mockActiveWorkerRepository = new Mock<IRepository<ActiveWorker>>();
        
        var options = new DbContextOptionsBuilder<PoolTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new PoolTrackerDbContext(options);
        
        _service = new WorkerService(
            _mockRepository.Object,
            _mockActiveWorkerRepository.Object,
            _context);
    }

    [Fact]
    public async Task GetAllWorkersAsync_ShouldReturnAllWorkers_WhenActiveOnlyIsFalse()
    {
        // Arrange
        var workers = new List<Worker>
        {
            new Worker { Id = 1, WorkerId = "W0001", Name = "João Silva", Role = WorkerRole.NadadorSalvador, IsActive = true },
            new Worker { Id = 2, WorkerId = "W0002", Name = "Maria Santos", Role = WorkerRole.Bar, IsActive = false }
        };
        _context.Workers.AddRange(workers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllWorkersAsync(activeOnly: false);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(w => w.Name == "João Silva");
        result.Should().Contain(w => w.Name == "Maria Santos");
    }

    [Fact]
    public async Task GetAllWorkersAsync_ShouldReturnOnlyActiveWorkers_WhenActiveOnlyIsTrue()
    {
        // Arrange
        var workers = new List<Worker>
        {
            new Worker { Id = 1, WorkerId = "W0001", Name = "João Silva", Role = WorkerRole.NadadorSalvador, IsActive = true },
            new Worker { Id = 2, WorkerId = "W0002", Name = "Maria Santos", Role = WorkerRole.Bar, IsActive = false }
        };
        _context.Workers.AddRange(workers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllWorkersAsync(activeOnly: true);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(w => w.Name == "João Silva");
        result.Should().NotContain(w => w.Name == "Maria Santos");
    }

    [Fact]
    public async Task GetWorkerByIdAsync_ShouldReturnWorker_WhenExists()
    {
        // Arrange
        var worker = new Worker
        {
            Id = 1,
            WorkerId = "W0001",
            Name = "João Silva",
            Role = WorkerRole.NadadorSalvador,
            IsActive = true
        };
        _context.Workers.Add(worker);
        await _context.SaveChangesAsync();
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(worker);

        // Act
        var result = await _service.GetWorkerByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("João Silva");
        result.WorkerId.Should().Be("W0001");
    }

    [Fact]
    public async Task GetWorkerByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Worker?)null);

        // Act
        var result = await _service.GetWorkerByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateWorkerAsync_ShouldCreateWorker_WithGeneratedWorkerId()
    {
        // Arrange
        var request = new CreateWorkerRequest
        {
            Name = "Novo Trabalhador",
            Role = WorkerRole.NadadorSalvador
        };

        // Act
        var result = await _service.CreateWorkerAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Novo Trabalhador");
        result.WorkerId.Should().StartWith("W");
        result.Role.Should().Be("nadador_salvador");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Worker>()), Times.Once);
    }

    [Fact]
    public async Task ActivateShiftAsync_ShouldCreateActiveWorker_WhenWorkerExists()
    {
        // Arrange
        var worker = new Worker
        {
            Id = 1,
            WorkerId = "W0001",
            Name = "João Silva",
            Role = WorkerRole.NadadorSalvador,
            IsActive = true
        };
        _context.Workers.Add(worker);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.ActivateShiftAsync("W0001", ShiftType.Manha);

        // Assert
        result.Should().BeTrue();
        var activeWorker = await _context.ActiveWorkers.FirstOrDefaultAsync(aw => aw.WorkerId == "W0001");
        activeWorker.Should().NotBeNull();
        activeWorker!.ShiftType.Should().Be(ShiftType.Manha);
    }

    [Fact]
    public async Task ActivateShiftAsync_ShouldReturnFalse_WhenWorkerNotExists()
    {
        // Act
        var result = await _service.ActivateShiftAsync("W9999", ShiftType.Manha);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeactivateShiftAsync_ShouldEndActiveShift_WhenShiftExists()
    {
        // Arrange
        var worker = new Worker
        {
            Id = 1,
            WorkerId = "W0001",
            Name = "João Silva",
            Role = WorkerRole.NadadorSalvador,
            IsActive = true
        };
        _context.Workers.Add(worker);
        
        var activeWorker = new ActiveWorker
        {
            WorkerId = "W0001",
            Role = WorkerRole.NadadorSalvador,
            ShiftType = ShiftType.Manha,
            StartTime = DateTime.UtcNow.AddHours(-2)
        };
        _context.ActiveWorkers.Add(activeWorker);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeactivateShiftAsync("W0001");

        // Assert
        result.Should().BeTrue();
        var updatedActiveWorker = await _context.ActiveWorkers.FirstOrDefaultAsync(aw => aw.WorkerId == "W0001" && aw.EndTime == null);
        updatedActiveWorker.Should().BeNull();
    }

    [Fact]
    public async Task DeactivateAllWorkersAsync_ShouldEndAllActiveShifts()
    {
        // Arrange
        var worker1 = new Worker { Id = 1, WorkerId = "W0001", Name = "João", Role = WorkerRole.NadadorSalvador, IsActive = true };
        var worker2 = new Worker { Id = 2, WorkerId = "W0002", Name = "Maria", Role = WorkerRole.Bar, IsActive = true };
        _context.Workers.AddRange(worker1, worker2);
        
        var activeWorkers = new List<ActiveWorker>
        {
            new ActiveWorker { WorkerId = "W0001", Role = WorkerRole.NadadorSalvador, ShiftType = ShiftType.Manha, StartTime = DateTime.UtcNow },
            new ActiveWorker { WorkerId = "W0002", Role = WorkerRole.Bar, ShiftType = ShiftType.Tarde, StartTime = DateTime.UtcNow }
        };
        _context.ActiveWorkers.AddRange(activeWorkers);
        await _context.SaveChangesAsync();

        // Act
        await _service.DeactivateAllWorkersAsync();

        // Assert
        var remainingActive = await _context.ActiveWorkers.CountAsync(aw => aw.EndTime == null);
        remainingActive.Should().Be(0);
    }
}

