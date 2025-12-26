using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Services;

public interface IJwtService
{
    Task<LoginResponse?> GenerateTokenAsync(string pin);
    Task<LoginResponse?> RefreshTokenAsync(string refreshToken);
}

