using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Services;

public interface IPoolService
{
    Task<PoolStatusDto> GetStatusAsync();
    Task<PoolStatusDto> EnterAsync();
    Task<PoolStatusDto> ExitAsync();
    Task<PoolStatusDto> SetCountAsync(int value);
    Task<PoolStatusDto> SetCapacityAsync(int value);
    Task<PoolStatusDto> SetOpenStatusAsync(bool isOpen);
    Task<PoolStatusDto> ResetAsync();
}

