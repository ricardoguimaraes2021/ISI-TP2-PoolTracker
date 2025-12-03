namespace PoolTracker.Core.Entities;

public class PoolStatus
{
    public int Id { get; set; }
    public int CurrentCount { get; set; } = 0;
    public int MaxCapacity { get; set; } = 120;
    public bool IsOpen { get; set; } = true;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public string LocationName { get; set; } = "Piscina Municipal da Sobreposta";
    public string Address { get; set; } = "R. da Piscina 22, 4715-553 Sobreposta";
    public string Phone { get; set; } = "253 636 948";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

