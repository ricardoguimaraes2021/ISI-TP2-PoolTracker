using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;

namespace PoolTracker.API.Services;

public class WorkerService : IWorkerService
{
    private readonly IRepository<Worker> _repository;
    private readonly IRepository<ActiveWorker> _activeWorkerRepository;
    private readonly PoolTrackerDbContext _context;

    public WorkerService(
        IRepository<Worker> repository,
        IRepository<ActiveWorker> activeWorkerRepository,
        PoolTrackerDbContext context)
    {
        _repository = repository;
        _activeWorkerRepository = activeWorkerRepository;
        _context = context;
    }

    private async Task<string> GenerateWorkerIdAsync()
    {
        // Get the last worker ID that starts with "W"
        // We avoid .Length check in LINQ as it might cause translation issues in some providers
        var lastWorker = await _context.Workers
            .Where(w => w.WorkerId.StartsWith("W"))
            .OrderByDescending(w => w.WorkerId)
            .FirstOrDefaultAsync();

        int nextId = 1;
        if (lastWorker != null)
        {
            // Try to parse the numeric part (everything after 'W')
            if (lastWorker.WorkerId.Length > 1 && int.TryParse(lastWorker.WorkerId.Substring(1), out int lastId))
            {
                nextId = lastId + 1;
            }
        }

        return $"W{nextId:D4}";
    }

    private ShiftType DetermineShiftType()
    {
        var hour = DateTime.UtcNow.Hour;
        return (hour >= 9 && hour < 14) ? ShiftType.Manha : ShiftType.Tarde;
    }

    public async Task<List<WorkerDto>> GetAllWorkersAsync(bool activeOnly = false)
    {
        var query = _context.Workers
            .GroupJoin(
                _context.ActiveWorkers.Where(aw => aw.EndTime == null),
                w => w.WorkerId,
                aw => aw.WorkerId,
                (w, aws) => new { Worker = w, ActiveWorkers = aws })
            .SelectMany(
                x => x.ActiveWorkers.DefaultIfEmpty(),
                (x, aw) => new { x.Worker, OnShift = aw != null });

        if (activeOnly)
        {
            query = query.Where(x => x.Worker.IsActive);
        }

        var results = await query
            .OrderBy(x => x.Worker.Name)
            .ToListAsync();

        return results.Select(x => new WorkerDto
        {
            Id = x.Worker.Id,
            WorkerId = x.Worker.WorkerId,
            Name = x.Worker.Name,
            Role = x.Worker.Role.ToString(),
            IsActive = x.Worker.IsActive,
            OnShift = x.OnShift,
            CreatedAt = x.Worker.CreatedAt,
            UpdatedAt = x.Worker.UpdatedAt
        }).ToList();
    }

    public async Task<WorkerDto?> GetWorkerByIdAsync(int id)
    {
        var worker = await _repository.GetByIdAsync(id);
        if (worker == null) return null;

        var onShift = await _context.ActiveWorkers
            .AnyAsync(aw => aw.WorkerId == worker.WorkerId && aw.EndTime == null);

        return new WorkerDto
        {
            Id = worker.Id,
            WorkerId = worker.WorkerId,
            Name = worker.Name,
            Role = worker.Role.ToString(),
            IsActive = worker.IsActive,
            OnShift = onShift,
            CreatedAt = worker.CreatedAt,
            UpdatedAt = worker.UpdatedAt
        };
    }

    public async Task<WorkerDto?> GetWorkerByWorkerIdAsync(string workerId)
    {
        var worker = await _context.Workers
            .FirstOrDefaultAsync(w => w.WorkerId == workerId);
        
        if (worker == null) return null;

        var onShift = await _context.ActiveWorkers
            .AnyAsync(aw => aw.WorkerId == workerId && aw.EndTime == null);

        return new WorkerDto
        {
            Id = worker.Id,
            WorkerId = worker.WorkerId,
            Name = worker.Name,
            Role = worker.Role.ToString(),
            IsActive = worker.IsActive,
            OnShift = onShift,
            CreatedAt = worker.CreatedAt,
            UpdatedAt = worker.UpdatedAt
        };
    }

    public async Task<WorkerDto> CreateWorkerAsync(CreateWorkerRequest request)
    {
        var workerId = request.WorkerId ?? await GenerateWorkerIdAsync();

        var worker = new Worker
        {
            WorkerId = workerId,
            Name = request.Name,
            Role = request.Role,
            IsActive = true
        };

        await _repository.AddAsync(worker);

        return new WorkerDto
        {
            Id = worker.Id,
            WorkerId = worker.WorkerId,
            Name = worker.Name,
            Role = worker.Role.ToString(),
            IsActive = worker.IsActive,
            OnShift = false,
            CreatedAt = worker.CreatedAt,
            UpdatedAt = worker.UpdatedAt
        };
    }

    public async Task<WorkerDto?> UpdateWorkerAsync(string workerId, UpdateWorkerRequest request)
    {
        var worker = await _context.Workers
            .FirstOrDefaultAsync(w => w.WorkerId == workerId);
        
        if (worker == null) return null;

        if (request.Name != null) worker.Name = request.Name;
        if (request.Role.HasValue) worker.Role = request.Role.Value;
        if (request.IsActive.HasValue) worker.IsActive = request.IsActive.Value;
        
        worker.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(worker);

        return await GetWorkerByWorkerIdAsync(workerId);
    }

    public async Task<bool> DeleteWorkerAsync(string workerId)
    {
        var worker = await _context.Workers
            .FirstOrDefaultAsync(w => w.WorkerId == workerId);
        
        if (worker == null) return false;

        worker.IsActive = false;
        worker.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(worker);

        return true;
    }

    public async Task<bool> ActivateShiftAsync(string workerId, ShiftType shiftType)
    {
        // Verifica se já está ativo
        var existing = await _context.ActiveWorkers
            .FirstOrDefaultAsync(aw => aw.WorkerId == workerId && aw.EndTime == null);
        
        if (existing != null) return true; // Já está ativo

        var worker = await _context.Workers
            .FirstOrDefaultAsync(w => w.WorkerId == workerId);
        
        if (worker == null) return false;

        var activeWorker = new ActiveWorker
        {
            WorkerId = workerId,
            Role = worker.Role,
            ShiftType = shiftType,
            StartTime = DateTime.UtcNow
        };

        await _activeWorkerRepository.AddAsync(activeWorker);
        return true;
    }

    public async Task<bool> DeactivateShiftAsync(string workerId)
    {
        var activeWorker = await _context.ActiveWorkers
            .FirstOrDefaultAsync(aw => aw.WorkerId == workerId && aw.EndTime == null);
        
        if (activeWorker == null) return false;

        activeWorker.EndTime = DateTime.UtcNow;
        await _activeWorkerRepository.UpdateAsync(activeWorker);

        return true;
    }

    public async Task DeactivateAllWorkersAsync()
    {
        var activeWorkers = await _context.ActiveWorkers
            .Where(aw => aw.EndTime == null)
            .ToListAsync();

        foreach (var activeWorker in activeWorkers)
        {
            activeWorker.EndTime = DateTime.UtcNow;
            await _activeWorkerRepository.UpdateAsync(activeWorker);
        }
    }

    public async Task<ActiveWorkersResponse> GetActiveWorkersAsync()
    {
        var activeWorkers = await _context.ActiveWorkers
            .Include(aw => aw.Worker)
            .Where(aw => aw.EndTime == null)
            .OrderBy(aw => aw.Role)
            .ThenBy(aw => aw.Worker!.Name)
            .ToListAsync();

        var counts = activeWorkers
            .GroupBy(aw => aw.Role.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var workers = activeWorkers.Select(aw => new WorkerDto
        {
            Id = aw.Worker!.Id,
            WorkerId = aw.WorkerId,
            Name = aw.Worker.Name,
            Role = aw.Role.ToString(),
            IsActive = aw.Worker.IsActive,
            OnShift = true,
            CreatedAt = aw.Worker.CreatedAt,
            UpdatedAt = aw.Worker.UpdatedAt
        }).ToList();

        return new ActiveWorkersResponse
        {
            Counts = counts,
            Workers = workers
        };
    }

    public async Task<Dictionary<string, int>> GetActiveWorkersCountAsync()
    {
        var counts = await _context.ActiveWorkers
            .Where(aw => aw.EndTime == null)
            .GroupBy(aw => aw.Role.ToString())
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        return counts;
    }
}

