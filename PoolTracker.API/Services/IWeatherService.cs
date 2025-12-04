using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Services;

public interface IWeatherService
{
    Task<WeatherInfoDto?> GetCurrentWeatherAsync();
}

