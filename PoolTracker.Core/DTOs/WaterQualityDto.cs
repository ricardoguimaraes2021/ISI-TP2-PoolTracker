using PoolTracker.Core.Entities;

namespace PoolTracker.Core.DTOs;

public class WaterQualityDto
{
    public int Id { get; set; }
    public string PoolType { get; set; } = string.Empty;
    public decimal PhLevel { get; set; }
    public decimal Temperature { get; set; }
    public DateTime MeasuredAt { get; set; }
    public string? Notes { get; set; }
}

public class RecordMeasurementRequest
{
    public PoolType PoolType { get; set; }
    public decimal PhLevel { get; set; }
    public decimal Temperature { get; set; }
    public string? Notes { get; set; }
}

public class CurrentMeasurementsResponse
{
    public WaterQualityDto? Criancas { get; set; }
    public WaterQualityDto? Adultos { get; set; }
}

