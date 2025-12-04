using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Services;

public interface IStatisticsService
{
    Task<VisitorsStatisticsDto> GetVisitorsStatisticsAsync(int days = 7);
    Task<WorkersStatisticsDto> GetWorkersStatisticsAsync(DateTime startDate, DateTime endDate);
    Task<OccupancyStatisticsDto> GetOccupancyStatisticsAsync(DateTime startDate, DateTime endDate);
}

