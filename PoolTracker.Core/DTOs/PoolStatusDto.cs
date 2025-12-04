namespace PoolTracker.Core.DTOs;

public class PoolStatusDto
{
    public int CurrentCount { get; set; }
    public int MaxCapacity { get; set; }
    public bool IsOpen { get; set; }
    public DateTime LastUpdated { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string TodayOpeningHours { get; set; } = string.Empty;
}

public class SetCountRequest
{
    public int Value { get; set; }
}

public class SetCapacityRequest
{
    public int Value { get; set; }
}

public class SetOpenStatusRequest
{
    public bool IsOpen { get; set; }
}

