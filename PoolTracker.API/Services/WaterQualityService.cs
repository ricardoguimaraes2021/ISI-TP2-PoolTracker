using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;

namespace PoolTracker.API.Services;

public class WaterQualityService : IWaterQualityService
{
    private readonly IRepository<WaterQuality> _repository;
    private readonly PoolTrackerDbContext _context;

    public WaterQualityService(IRepository<WaterQuality> repository, PoolTrackerDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<WaterQualityDto> RecordMeasurementAsync(RecordMeasurementRequest request)
    {
        var measurement = new WaterQuality
        {
            PoolType = request.PoolType,
            PhLevel = request.PhLevel,
            Temperature = request.Temperature,
            Notes = request.Notes,
            MeasuredAt = DateTime.UtcNow
        };

        await _repository.AddAsync(measurement);

        return new WaterQualityDto
        {
            Id = measurement.Id,
            PoolType = measurement.PoolType.ToString(),
            PhLevel = measurement.PhLevel,
            Temperature = measurement.Temperature,
            MeasuredAt = measurement.MeasuredAt,
            Notes = measurement.Notes
        };
    }

    public async Task<WaterQualityDto?> GetMeasurementByIdAsync(int id)
    {
        var measurement = await _repository.GetByIdAsync(id);
        if (measurement == null) return null;

        return new WaterQualityDto
        {
            Id = measurement.Id,
            PoolType = measurement.PoolType.ToString(),
            PhLevel = measurement.PhLevel,
            Temperature = measurement.Temperature,
            MeasuredAt = measurement.MeasuredAt,
            Notes = measurement.Notes
        };
    }

    public async Task<WaterQualityDto?> GetLatestMeasurementAsync(PoolType poolType)
    {
        var measurement = await _context.WaterQuality
            .Where(wq => wq.PoolType == poolType)
            .OrderByDescending(wq => wq.MeasuredAt)
            .FirstOrDefaultAsync();

        if (measurement == null) return null;

        return new WaterQualityDto
        {
            Id = measurement.Id,
            PoolType = measurement.PoolType.ToString(),
            PhLevel = measurement.PhLevel,
            Temperature = measurement.Temperature,
            MeasuredAt = measurement.MeasuredAt,
            Notes = measurement.Notes
        };
    }

    public async Task<CurrentMeasurementsResponse> GetCurrentMeasurementsAsync()
    {
        var criancas = await GetLatestMeasurementAsync(PoolType.Criancas);
        var adultos = await GetLatestMeasurementAsync(PoolType.Adultos);

        return new CurrentMeasurementsResponse
        {
            Criancas = criancas,
            Adultos = adultos
        };
    }

    public async Task<List<WaterQualityDto>> GetMeasurementsByDateRangeAsync(DateTime startDate, DateTime endDate, PoolType? poolType = null)
    {
        var query = _context.WaterQuality
            .Where(wq => wq.MeasuredAt.Date >= startDate.Date && wq.MeasuredAt.Date <= endDate.Date);

        if (poolType.HasValue)
        {
            query = query.Where(wq => wq.PoolType == poolType.Value);
        }

        var measurements = await query
            .OrderByDescending(wq => wq.MeasuredAt)
            .ToListAsync();

        return measurements.Select(m => new WaterQualityDto
        {
            Id = m.Id,
            PoolType = m.PoolType.ToString(),
            PhLevel = m.PhLevel,
            Temperature = m.Temperature,
            MeasuredAt = m.MeasuredAt,
            Notes = m.Notes
        }).ToList();
    }

    public async Task<bool> DeleteMeasurementAsync(int id)
    {
        var measurement = await _repository.GetByIdAsync(id);
        if (measurement == null) return false;

        await _repository.DeleteAsync(measurement);
        return true;
    }
}

