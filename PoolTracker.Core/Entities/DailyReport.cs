namespace PoolTracker.Core.Entities;

public class DailyReport
{
    public int Id { get; set; }
    public DateOnly ReportDate { get; set; }
    public int TotalVisitors { get; set; } = 0;
    public int MaxOccupancy { get; set; } = 0;
    public decimal? AvgOccupancy { get; set; }
    public DateTime? OpeningTime { get; set; }
    public DateTime ClosingTime { get; set; }
    public string? WaterQualityChildren { get; set; } // JSON
    public string? WaterQualityAdults { get; set; } // JSON
    public string? ActiveWorkersCount { get; set; } // JSON
    public string? CleaningRecords { get; set; } // JSON
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

