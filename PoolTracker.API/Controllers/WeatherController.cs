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
    /// <remarks>
    /// Retorna os dados meteorológicos atuais para a localização da piscina.
    /// Endpoint público, não requer autenticação.
    /// Integra com a API Open-Meteo para obter dados em tempo real.
    /// Os dados são cacheados por 5 minutos para evitar rate limiting.
    /// Inclui temperatura, velocidade do vento, descrição e ícone do tempo.
    /// </remarks>
    /// <returns>Dados meteorológicos atuais</returns>
    /// <response code="200">Dados meteorológicos retornados com sucesso</response>
    /// <response code="503">Falha ao obter dados da API externa</response>
    [HttpGet("current")]
    [ProducesResponseType(typeof(PoolTracker.Core.DTOs.WeatherInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> GetCurrentWeather()
    {
        var weather = await _weatherService.GetCurrentWeatherAsync();
        if (weather == null)
            return StatusCode(503, new { error = "Falha ao obter dados meteorológicos." });

        return Ok(weather);
    }
}

