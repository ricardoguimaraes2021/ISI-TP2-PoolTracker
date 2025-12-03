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
    /// Listar relatórios por período
    /// </summary>
    [HttpGet]
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
    /// Obter último relatório
    /// </summary>
    [HttpGet("latest")]
    public async Task<ActionResult<DailyReportDto>> GetLatest()
    {
        var report = await _reportService.GetLatestReportAsync();
        if (report == null) return NotFound();
        return Ok(report);
    }

    /// <summary>
    /// Obter relatório por data
    /// </summary>
    [HttpGet("{date}")]
    public async Task<ActionResult<DailyReportDto>> GetByDate(DateOnly date)
    {
        var report = await _reportService.GetReportByDateAsync(date);
        if (report == null) return NotFound();
        return Ok(report);
    }

    /// <summary>
    /// Gerar relatório diário
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<DailyReportDto>> GenerateReport([FromBody] GenerateReportRequest? request = null)
    {
        var report = await _reportService.GenerateDailyReportAsync(request?.ClosingTime);
        return CreatedAtAction(nameof(GetByDate), new { date = report.ReportDate }, report);
    }
}

