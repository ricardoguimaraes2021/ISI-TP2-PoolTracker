namespace PoolTracker.Core.Entities;

public class ActiveWorker
{
    public int Id { get; set; }
    public string WorkerId { get; set; } = string.Empty;
    public WorkerRole Role { get; set; }
    public ShiftType? ShiftType { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    
    // Navigation property
    public Worker? Worker { get; set; }
}

public enum ShiftType
{
    Manha,
    Tarde
}

