using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.Entities;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;
using PoolTracker.SOAP.Contracts;
using PoolTracker.SOAP.DataContracts;

namespace PoolTracker.SOAP.Services;

public class WorkerDataService : IWorkerDataService
{
    private readonly IRepository<Worker> _repository;
    private readonly PoolTrackerDbContext _context;

    public WorkerDataService(IRepository<Worker> repository, PoolTrackerDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public List<WorkerData> GetAllWorkers()
    {
        var workers = _context.Workers.ToList();
        return workers.Select(w => new WorkerData
        {
            Id = w.Id,
            WorkerId = w.WorkerId,
            Name = w.Name,
            Role = w.Role.ToString(),
            IsActive = w.IsActive,
            CreatedAt = w.CreatedAt,
            UpdatedAt = w.UpdatedAt
        }).ToList();
    }

    public WorkerData GetWorkerById(int id)
    {
        var worker = _repository.GetByIdAsync(id).Result;
        if (worker == null)
        {
            throw new InvalidOperationException($"Worker with ID {id} not found");
        }

        return new WorkerData
        {
            Id = worker.Id,
            WorkerId = worker.WorkerId,
            Name = worker.Name,
            Role = worker.Role.ToString(),
            IsActive = worker.IsActive,
            CreatedAt = worker.CreatedAt,
            UpdatedAt = worker.UpdatedAt
        };
    }

    public int CreateWorker(WorkerData workerData)
    {
        var worker = new Worker
        {
            WorkerId = workerData.WorkerId,
            Name = workerData.Name,
            Role = Enum.Parse<WorkerRole>(workerData.Role),
            IsActive = workerData.IsActive
        };

        _repository.AddAsync(worker).Wait();
        return worker.Id;
    }

    public void UpdateWorker(WorkerData workerData)
    {
        var worker = _context.Workers.FirstOrDefault(w => w.Id == workerData.Id);
        if (worker == null)
        {
            throw new InvalidOperationException($"Worker with ID {workerData.Id} not found");
        }

        worker.Name = workerData.Name;
        worker.Role = Enum.Parse<WorkerRole>(workerData.Role);
        worker.IsActive = workerData.IsActive;
        worker.UpdatedAt = DateTime.UtcNow;

        _repository.UpdateAsync(worker).Wait();
    }

    public void DeleteWorker(int id)
    {
        var worker = _repository.GetByIdAsync(id).Result;
        if (worker == null)
        {
            throw new InvalidOperationException($"Worker with ID {id} not found");
        }

        _repository.DeleteAsync(worker).Wait();
    }
}

