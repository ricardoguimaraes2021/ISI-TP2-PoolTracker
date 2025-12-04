using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.Entities;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;

namespace PoolTracker.API.Services;

public class VisitService : IVisitService
{
    private readonly IRepository<DailyVisitor> _repository;
    private readonly PoolTrackerDbContext _context;

    public VisitService(IRepository<DailyVisitor> repository, PoolTrackerDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task IncrementDailyVisitorsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var visitor = await _context.DailyVisitors
            .FirstOrDefaultAsync(v => v.VisitDate == today);

        if (visitor == null)
        {
            visitor = new DailyVisitor
            {
                VisitDate = today,
                TotalVisitors = 1
            };
            await _repository.AddAsync(visitor);
        }
        else
        {
            visitor.TotalVisitors++;
            visitor.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(visitor);
        }
    }

    public async Task<int> GetTodayVisitorsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var visitor = await _context.DailyVisitors
            .FirstOrDefaultAsync(v => v.VisitDate == today);

        return visitor?.TotalVisitors ?? 0;
    }
}

