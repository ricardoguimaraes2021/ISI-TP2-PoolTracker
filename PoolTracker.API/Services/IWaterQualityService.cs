using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;

namespace PoolTracker.API.Services;

public interface IWaterQualityService
{
    Task<WaterQualityDto> RecordMeasurementAsync(RecordMeasurementRequest request);
    Task<WaterQualityDto?> GetMeasurementByIdAsync(int id);
    Task<WaterQualityDto?> GetLatestMeasurementAsync(PoolType poolType);
    Task<CurrentMeasurementsResponse> GetCurrentMeasurementsAsync();
    Task<List<WaterQualityDto>> GetMeasurementsByDateRangeAsync(DateTime startDate, DateTime endDate, PoolType? poolType = null);
    Task<bool> DeleteMeasurementAsync(int id);
}

