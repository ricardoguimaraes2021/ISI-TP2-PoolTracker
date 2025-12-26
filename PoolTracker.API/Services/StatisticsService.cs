using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.DTOs;
using PoolTracker.Infrastructure.Data;

namespace PoolTracker.API.Services;

public class StatisticsService : IStatisticsService
{
    private readonly PoolTrackerDbContext _context;

    public StatisticsService(PoolTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<VisitorsStatisticsDto> GetVisitorsStatisticsAsync(int days = 7)
    {
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = endDate.AddDays(-days);

        var visitors = await _context.DailyVisitors
            .Where(v => v.VisitDate >= startDate && v.VisitDate <= endDate)
            .OrderBy(v => v.VisitDate)
            .ToListAsync();

        var data = visitors.Select(v => new VisitorDataPoint
        {
            Date = v.VisitDate,
            Visitors = v.TotalVisitors
        }).ToList();

        return new VisitorsStatisticsDto { Data = data };
    }

    public async Task<WorkersStatisticsDto> GetWorkersStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var shifts = await _context.ActiveWorkers
            .Where(aw => aw.StartTime >= startDate && aw.StartTime <= endDate && aw.EndTime != null)
            .GroupBy(aw => new { aw.WorkerId, aw.Worker!.Name })
            .Select(g => new WorkerShiftData
            {
                WorkerId = g.Key.WorkerId,
                Name = g.Key.Name ?? string.Empty,
                ShiftCount = g.Count()
            })
            .OrderByDescending(w => w.ShiftCount)
            .ToListAsync();

        return new WorkersStatisticsDto { Data = shifts };
    }

    public async Task<OccupancyStatisticsDto> GetOccupancyStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var start = DateOnly.FromDateTime(startDate);
        var end = DateOnly.FromDateTime(endDate);

        var reports = await _context.DailyReports
            .Where(r => r.ReportDate >= start && r.ReportDate <= end)
            .ToListAsync();

        if (!reports.Any())
        {
            return new OccupancyStatisticsDto
            {
                AverageOccupancy = 0,
                MaxOccupancy = 0,
                MinOccupancy = 0
            };
        }

        var avgOccupancy = reports.Where(r => r.AvgOccupancy.HasValue)
            .Average(r => (double)r.AvgOccupancy!.Value);
        
        var maxOccupancy = reports.Max(r => r.MaxOccupancy);
        var minOccupancy = reports.Min(r => r.MaxOccupancy);

        return new OccupancyStatisticsDto
        {
            AverageOccupancy = (decimal)avgOccupancy,
            MaxOccupancy = maxOccupancy,
            MinOccupancy = minOccupancy
        };
    }
}

