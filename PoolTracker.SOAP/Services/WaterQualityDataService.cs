using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.Entities;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;
using PoolTracker.SOAP.Contracts;
using PoolTracker.SOAP.DataContracts;

namespace PoolTracker.SOAP.Services;

public class WaterQualityDataService : IWaterQualityDataService
{
    private readonly IRepository<WaterQuality> _repository;
    private readonly PoolTrackerDbContext _context;

    public WaterQualityDataService(IRepository<WaterQuality> repository, PoolTrackerDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public List<WaterQualityData> GetHistory(string poolType)
    {
        var poolTypeEnum = Enum.Parse<PoolType>(poolType, true);
        var measurements = _context.WaterQuality
            .Where(wq => wq.PoolType == poolTypeEnum)
            .OrderByDescending(wq => wq.MeasuredAt)
            .ToList();

        return measurements.Select(m => new WaterQualityData
        {
            Id = m.Id,
            PoolType = m.PoolType.ToString(),
            PhLevel = m.PhLevel,
            Temperature = m.Temperature,
            MeasuredAt = m.MeasuredAt,
            Notes = m.Notes
        }).ToList();
    }

    public WaterQualityData GetLatest(string poolType)
    {
        var poolTypeEnum = Enum.Parse<PoolType>(poolType, true);
        var measurement = _context.WaterQuality
            .Where(wq => wq.PoolType == poolTypeEnum)
            .OrderByDescending(wq => wq.MeasuredAt)
            .FirstOrDefault();

        if (measurement == null)
        {
            throw new InvalidOperationException($"No measurements found for pool type {poolType}");
        }

        return new WaterQualityData
        {
            Id = measurement.Id,
            PoolType = measurement.PoolType.ToString(),
            PhLevel = measurement.PhLevel,
            Temperature = measurement.Temperature,
            MeasuredAt = measurement.MeasuredAt,
            Notes = measurement.Notes
        };
    }

    public void RecordMeasurement(WaterQualityData measurementData)
    {
        var measurement = new WaterQuality
        {
            PoolType = Enum.Parse<PoolType>(measurementData.PoolType, true),
            PhLevel = measurementData.PhLevel,
            Temperature = measurementData.Temperature,
            Notes = measurementData.Notes,
            MeasuredAt = DateTime.UtcNow
        };

        _repository.AddAsync(measurement).Wait();
    }
}

