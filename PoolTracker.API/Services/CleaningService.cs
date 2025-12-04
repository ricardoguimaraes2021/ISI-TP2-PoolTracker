using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;

namespace PoolTracker.API.Services;

public class CleaningService : ICleaningService
{
    private readonly IRepository<Cleaning> _repository;
    private readonly PoolTrackerDbContext _context;

    public CleaningService(IRepository<Cleaning> repository, PoolTrackerDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<CleaningDto> RecordCleaningAsync(RecordCleaningRequest request)
    {
        var cleaning = new Cleaning
        {
            CleaningType = request.CleaningType,
            Notes = request.Notes,
            CleanedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(cleaning);

        return new CleaningDto
        {
            Id = cleaning.Id,
            CleaningType = cleaning.CleaningType.ToString(),
            CleanedAt = cleaning.CleanedAt,
            Notes = cleaning.Notes
        };
    }

    public async Task<CleaningDto?> GetCleaningByIdAsync(int id)
    {
        var cleaning = await _repository.GetByIdAsync(id);
        if (cleaning == null) return null;

        return new CleaningDto
        {
            Id = cleaning.Id,
            CleaningType = cleaning.CleaningType.ToString(),
            CleanedAt = cleaning.CleanedAt,
            Notes = cleaning.Notes
        };
    }

    public async Task<CleaningDto?> GetLastCleaningAsync(CleaningType cleaningType)
    {
        var cleaning = await _context.Cleanings
            .Where(c => c.CleaningType == cleaningType)
            .OrderByDescending(c => c.CleanedAt)
            .FirstOrDefaultAsync();

        if (cleaning == null) return null;

        return new CleaningDto
        {
            Id = cleaning.Id,
            CleaningType = cleaning.CleaningType.ToString(),
            CleanedAt = cleaning.CleanedAt,
            Notes = cleaning.Notes
        };
    }

    public async Task<LastCleaningsResponse> GetLastCleaningsAsync()
    {
        var balnearios = await GetLastCleaningAsync(CleaningType.Balnearios);
        var wc = await GetLastCleaningAsync(CleaningType.Wc);

        return new LastCleaningsResponse
        {
            Balnearios = balnearios,
            Wc = wc
        };
    }

    public async Task<List<CleaningDto>> GetRecentCleaningsAsync(int limit = 10)
    {
        var cleanings = await _context.Cleanings
            .OrderByDescending(c => c.CleanedAt)
            .Take(limit)
            .ToListAsync();

        return cleanings.Select(c => new CleaningDto
        {
            Id = c.Id,
            CleaningType = c.CleaningType.ToString(),
            CleanedAt = c.CleanedAt,
            Notes = c.Notes
        }).ToList();
    }

    public async Task<bool> DeleteCleaningAsync(int id)
    {
        var cleaning = await _repository.GetByIdAsync(id);
        if (cleaning == null) return false;

        await _repository.DeleteAsync(cleaning);
        return true;
    }
}

