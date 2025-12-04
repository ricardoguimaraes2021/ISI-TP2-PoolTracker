namespace PoolTracker.Core.Entities;

public class Cleaning
{
    public int Id { get; set; }
    public CleaningType CleaningType { get; set; }
    public DateTime CleanedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}

public enum CleaningType
{
    Balnearios,
    Wc
}

