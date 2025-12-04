namespace PoolTracker.Core.Entities;

public class Worker
{
    public int Id { get; set; }
    public string WorkerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public WorkerRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public ICollection<ActiveWorker> ActiveWorkers { get; set; } = new List<ActiveWorker>();
}

public enum WorkerRole
{
    NadadorSalvador,
    Bar,
    Vigilante,
    Bilheteira
}

