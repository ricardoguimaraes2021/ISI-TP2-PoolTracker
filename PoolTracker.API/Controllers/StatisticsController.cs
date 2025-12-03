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
    /// Obter estatísticas de visitantes
    /// </summary>
    /// <remarks>
    /// Retorna estatísticas de visitantes para um período especificado.
    /// Requer autenticação JWT.
    /// Por padrão, retorna dados dos últimos 7 dias.
    /// Útil para gerar gráficos de fluxo de visitantes.
    /// </remarks>
    /// <param name="days">Número de dias a incluir (padrão: 7)</param>
    /// <returns>Estatísticas de visitantes com dados diários</returns>
    /// <response code="200">Estatísticas retornadas com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("visitors")]
    [ProducesResponseType(typeof(VisitorsStatisticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<VisitorsStatisticsDto>> GetVisitorsStatistics([FromQuery] int days = 7)
    {
        var statistics = await _statisticsService.GetVisitorsStatisticsAsync(days);
        return Ok(statistics);
    }

    /// <summary>
    /// Obter estatísticas de turnos por trabalhador
    /// </summary>
    /// <remarks>
    /// Retorna estatísticas de turnos trabalhados por cada trabalhador.
    /// Requer autenticação JWT.
    /// Por padrão, retorna dados dos últimos 30 dias.
    /// Útil para análise de distribuição de turnos e horas trabalhadas.
    /// </remarks>
    /// <param name="startDate">Data de início (padrão: 30 dias atrás)</param>
    /// <param name="endDate">Data de fim (padrão: hoje)</param>
    /// <returns>Estatísticas de turnos por trabalhador</returns>
    /// <response code="200">Estatísticas retornadas com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("workers")]
    [ProducesResponseType(typeof(WorkersStatisticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    /// <remarks>
    /// Retorna estatísticas agregadas de ocupação da piscina.
    /// Requer autenticação JWT.
    /// Por padrão, retorna dados dos últimos 30 dias.
    /// Inclui ocupação média, máxima e mínima.
    /// Útil para análise de tendências de ocupação.
    /// </remarks>
    /// <param name="startDate">Data de início (padrão: 30 dias atrás)</param>
    /// <param name="endDate">Data de fim (padrão: hoje)</param>
    /// <returns>Estatísticas de ocupação</returns>
    /// <response code="200">Estatísticas retornadas com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("occupancy")]
    [ProducesResponseType(typeof(OccupancyStatisticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

