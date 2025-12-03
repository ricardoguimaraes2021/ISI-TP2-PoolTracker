using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// Obter estatísticas de visitantes (últimos 7 dias por padrão)
    /// </summary>
    [HttpGet("visitors")]
    public async Task<ActionResult<VisitorsStatisticsDto>> GetVisitorsStatistics([FromQuery] int days = 7)
    {
        var statistics = await _statisticsService.GetVisitorsStatisticsAsync(days);
        return Ok(statistics);
    }

    /// <summary>
    /// Obter estatísticas de turnos por trabalhador
    /// </summary>
    [HttpGet("workers")]
    public async Task<ActionResult<WorkersStatisticsDto>> GetWorkersStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;
        
        var statistics = await _statisticsService.GetWorkersStatisticsAsync(start, end);
        return Ok(statistics);
    }

    /// <summary>
    /// Obter estatísticas de ocupação
    /// </summary>
    [HttpGet("occupancy")]
    public async Task<ActionResult<OccupancyStatisticsDto>> GetOccupancyStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;
        
        var statistics = await _statisticsService.GetOccupancyStatisticsAsync(start, end);
        return Ok(statistics);
    }
}

