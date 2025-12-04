using PoolTracker.Core.Entities;

namespace PoolTracker.Core.DTOs;

public class WorkerDto
{
    public int Id { get; set; }
    public string WorkerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool OnShift { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateWorkerRequest
{
    public string? WorkerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public WorkerRole Role { get; set; }
}

public class UpdateWorkerRequest
{
    public string? Name { get; set; }
    public WorkerRole? Role { get; set; }
    public bool? IsActive { get; set; }
}

public class ActivateShiftRequest
{
    public ShiftType ShiftType { get; set; }
}

public class ActiveWorkersResponse
{
    public Dictionary<string, int> Counts { get; set; } = new();
    public List<WorkerDto> Workers { get; set; } = new();
}

public class ShiftStatsDto
{
    public string WorkerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int Manha { get; set; }
    public int Tarde { get; set; }
    public int Total { get; set; }
}

public class WorkerShiftDto
{
    public int Id { get; set; }
    public string WorkerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ShiftType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

