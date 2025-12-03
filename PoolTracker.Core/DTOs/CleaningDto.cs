using PoolTracker.Core.Entities;

namespace PoolTracker.Core.DTOs;

public class CleaningDto
{
    public int Id { get; set; }
    public string CleaningType { get; set; } = string.Empty;
    public DateTime CleanedAt { get; set; }
    public string? Notes { get; set; }
}

public class RecordCleaningRequest
{
    public CleaningType CleaningType { get; set; }
    public string? Notes { get; set; }
}

public class LastCleaningsResponse
{
    public CleaningDto? Balnearios { get; set; }
    public CleaningDto? Wc { get; set; }
}

