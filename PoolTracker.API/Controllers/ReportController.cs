using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Listar relatórios diários por período
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de relatórios diários dentro de um período especificado.
    /// Requer autenticação JWT.
    /// Por padrão, retorna relatórios dos últimos 30 dias.
    /// Cada relatório inclui visitantes, ocupação, qualidade da água, trabalhadores e limpezas.
    /// </remarks>
    /// <param name="startDate">Data de início (padrão: 30 dias atrás)</param>
    /// <param name="endDate">Data de fim (padrão: hoje)</param>
    /// <returns>Lista de relatórios diários</returns>
    /// <response code="200">Lista de relatórios retornada com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<DailyReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<DailyReportDto>>> GetReports(
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null)
    {
        var start = startDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        var end = endDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        
        var reports = await _reportService.GetReportsByDateRangeAsync(start, end);
        return Ok(reports);
    }

    /// <summary>
    /// Obter último relatório diário gerado
    /// </summary>
    /// <remarks>
    /// Retorna o relatório diário mais recente.
    /// Requer autenticação JWT.
    /// Útil para visualizar rapidamente o último relatório gerado.
    /// </remarks>
    /// <returns>Último relatório diário</returns>
    /// <response code="200">Último relatório encontrado</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Nenhum relatório encontrado</response>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(DailyReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DailyReportDto>> GetLatest()
    {
        var report = await _reportService.GetLatestReportAsync();
        if (report == null) return NotFound();
        return Ok(report);
    }

    /// <summary>
    /// Obter relatório diário por data específica
    /// </summary>
    /// <remarks>
    /// Retorna o relatório diário para uma data específica.
    /// Requer autenticação JWT.
    /// O formato da data deve ser YYYY-MM-DD.
    /// </remarks>
    /// <param name="date">Data do relatório (formato: YYYY-MM-DD)</param>
    /// <returns>Relatório diário da data especificada</returns>
    /// <response code="200">Relatório encontrado</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Relatório não encontrado para a data especificada</response>
    [HttpGet("{date}")]
    [ProducesResponseType(typeof(DailyReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DailyReportDto>> GetByDate(DateOnly date)
    {
        var report = await _reportService.GetReportByDateAsync(date);
        if (report == null) return NotFound();
        return Ok(report);
    }

    /// <summary>
    /// Gerar relatório diário
    /// </summary>
    /// <remarks>
    /// Gera um relatório diário completo com todos os dados do dia.
    /// Requer autenticação JWT.
    /// O relatório inclui:
    /// - Total de visitantes
    /// - Ocupação máxima e média
    /// - Qualidade da água (crianças e adultos)
    /// - Contagem de trabalhadores ativos
    /// - Registos de limpezas
    /// Se já existir um relatório para a data atual, será atualizado.
    /// </remarks>
    /// <param name="request">Dados opcionais (horário de fecho)</param>
    /// <returns>Relatório diário gerado</returns>
    /// <response code="201">Relatório gerado com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(DailyReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DailyReportDto>> GenerateReport([FromBody] GenerateReportRequest? request = null)
    {
        var report = await _reportService.GenerateDailyReportAsync(request?.ClosingTime);
        return CreatedAtAction(nameof(GetByDate), new { date = report.ReportDate }, report);
    }
}

