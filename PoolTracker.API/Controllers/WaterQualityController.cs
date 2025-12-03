using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WaterQualityController : ControllerBase
{
    private readonly IWaterQualityService _waterQualityService;

    public WaterQualityController(IWaterQualityService waterQualityService)
    {
        _waterQualityService = waterQualityService;
    }

    /// <summary>
    /// Obter histórico de medições
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<WaterQualityDto>>> GetHistory(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] PoolType? poolType = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;
        
        var measurements = await _waterQualityService.GetMeasurementsByDateRangeAsync(start, end, poolType);
        return Ok(measurements);
    }

    /// <summary>
    /// Obter última medição (público)
    /// </summary>
    [HttpGet("latest")]
    public async Task<ActionResult<CurrentMeasurementsResponse>> GetLatest()
    {
        var measurements = await _waterQualityService.GetCurrentMeasurementsAsync();
        return Ok(measurements);
    }

    /// <summary>
    /// Registar nova medição
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<WaterQualityDto>> RecordMeasurement([FromBody] RecordMeasurementRequest request)
    {
        var measurement = await _waterQualityService.RecordMeasurementAsync(request);
        return CreatedAtAction(nameof(GetMeasurementById), new { id = measurement.Id }, measurement);
    }

    /// <summary>
    /// Obter medição por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<WaterQualityDto>> GetMeasurementById(int id)
    {
        var measurement = await _waterQualityService.GetMeasurementByIdAsync(id);
        if (measurement == null) return NotFound();
        return Ok(measurement);
    }

    /// <summary>
    /// Eliminar medição
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _waterQualityService.DeleteMeasurementAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}

