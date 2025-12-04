namespace PoolTracker.Core.DTOs;

public class VisitorsStatisticsDto
{
    public List<VisitorDataPoint> Data { get; set; } = new();
}

public class VisitorDataPoint
{
    public DateOnly Date { get; set; }
    public int Visitors { get; set; }
}

public class WorkersStatisticsDto
{
    public List<WorkerShiftData> Data { get; set; } = new();
}

public class WorkerShiftData
{
    public string WorkerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ShiftCount { get; set; }
}

public class OccupancyStatisticsDto
{
    public decimal AverageOccupancy { get; set; }
    public int MaxOccupancy { get; set; }
    public int MinOccupancy { get; set; }
}

