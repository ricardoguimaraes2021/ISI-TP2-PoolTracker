using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.Entities;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;
using PoolTracker.SOAP.Contracts;
using PoolTracker.SOAP.DataContracts;

namespace PoolTracker.SOAP.Services;

public class PoolDataService : IPoolDataService
{
    private readonly IRepository<PoolStatus> _repository;
    private readonly PoolTrackerDbContext _context;

    public PoolDataService(IRepository<PoolStatus> repository, PoolTrackerDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public PoolStatusData GetPoolStatus()
    {
        var status = _context.PoolStatus.FirstOrDefault();
        
        if (status == null)
        {
            // Criar estado inicial se não existir
            status = new PoolStatus
            {
                CurrentCount = 0,
                MaxCapacity = 120,
                IsOpen = true,
                LocationName = "Piscina Municipal da Sobreposta",
                Address = "R. da Piscina 22, 4715-553 Sobreposta",
                Phone = "253 636 948"
            };
            _repository.AddAsync(status).Wait();
        }

        return new PoolStatusData
        {
            CurrentCount = status.CurrentCount,
            MaxCapacity = status.MaxCapacity,
            IsOpen = status.IsOpen,
            LastUpdated = status.LastUpdated,
            LocationName = status.LocationName,
            Address = status.Address,
            Phone = status.Phone
        };
    }

    public void UpdatePoolStatus(PoolStatusData statusData)
    {
        var status = _context.PoolStatus.FirstOrDefault();
        if (status == null) return;

        status.CurrentCount = statusData.CurrentCount;
        status.MaxCapacity = statusData.MaxCapacity;
        status.IsOpen = statusData.IsOpen;
        status.LastUpdated = DateTime.UtcNow;
        status.LocationName = statusData.LocationName;
        status.Address = statusData.Address;
        status.Phone = statusData.Phone;

        _repository.UpdateAsync(status).Wait();
    }

    public void IncrementCount()
    {
        var status = _context.PoolStatus.FirstOrDefault();
        if (status == null) return;

        if (status.CurrentCount < status.MaxCapacity && status.IsOpen)
        {
            status.CurrentCount++;
            status.LastUpdated = DateTime.UtcNow;
            _repository.UpdateAsync(status).Wait();
        }
    }

    public void DecrementCount()
    {
        var status = _context.PoolStatus.FirstOrDefault();
        if (status == null) return;

        if (status.CurrentCount > 0)
        {
            status.CurrentCount--;
            status.LastUpdated = DateTime.UtcNow;
            _repository.UpdateAsync(status).Wait();
        }
    }
}

