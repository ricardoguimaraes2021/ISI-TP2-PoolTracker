using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    /// <summary>
    /// Obter meteorologia atual (público)
    /// </summary>
    [HttpGet("current")]
    public async Task<ActionResult> GetCurrentWeather()
    {
        var weather = await _weatherService.GetCurrentWeatherAsync();
        if (weather == null)
            return StatusCode(503, new { error = "Falha ao obter dados meteorológicos." });

        return Ok(weather);
    }
}

