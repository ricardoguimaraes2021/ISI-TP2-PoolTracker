using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;

namespace PoolTracker.API.Services;

public interface ICleaningService
{
    Task<CleaningDto> RecordCleaningAsync(RecordCleaningRequest request);
    Task<CleaningDto?> GetCleaningByIdAsync(int id);
    Task<CleaningDto?> GetLastCleaningAsync(CleaningType cleaningType);
    Task<LastCleaningsResponse> GetLastCleaningsAsync();
    Task<List<CleaningDto>> GetRecentCleaningsAsync(int limit = 10);
    Task<bool> DeleteCleaningAsync(int id);
}

