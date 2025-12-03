namespace PoolTracker.Core.Entities;

public class WaterQuality
{
    public int Id { get; set; }
    public PoolType PoolType { get; set; }
    public decimal PhLevel { get; set; }
    public decimal Temperature { get; set; }
    public DateTime MeasuredAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}

public enum PoolType
{
    Criancas,
    Adultos
}

