namespace PoolTracker.Core.DTOs;

public class DailyReportDto
{
    public int Id { get; set; }
    public DateOnly ReportDate { get; set; }
    public int TotalVisitors { get; set; }
    public int MaxOccupancy { get; set; }
    public decimal? AvgOccupancy { get; set; }
    public DateTime? OpeningTime { get; set; }
    public DateTime ClosingTime { get; set; }
    public Dictionary<string, object>? WaterQualityChildren { get; set; }
    public Dictionary<string, object>? WaterQualityAdults { get; set; }
    public Dictionary<string, int>? ActiveWorkersCount { get; set; }
    public List<Dictionary<string, object>>? CleaningRecords { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GenerateReportRequest
{
    public DateTime? ClosingTime { get; set; }
}

