using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Services;

public interface IReportService
{
    Task<DailyReportDto> GenerateDailyReportAsync(DateTime? closingTime = null);
    Task<DailyReportDto?> GetReportByDateAsync(DateOnly date);
    Task<List<DailyReportDto>> GetReportsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<DailyReportDto?> GetLatestReportAsync();
}

