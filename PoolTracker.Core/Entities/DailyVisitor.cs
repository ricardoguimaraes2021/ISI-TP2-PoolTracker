namespace PoolTracker.Core.Entities;

public class DailyVisitor
{
    public int Id { get; set; }
    public DateOnly VisitDate { get; set; }
    public int TotalVisitors { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

