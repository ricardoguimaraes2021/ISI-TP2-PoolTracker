using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CleaningController : ControllerBase
{
    private readonly ICleaningService _cleaningService;

    public CleaningController(ICleaningService cleaningService)
    {
        _cleaningService = cleaningService;
    }

    /// <summary>
    /// Obter histórico de limpezas
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<CleaningDto>>> GetHistory([FromQuery] int limit = 10)
    {
        var cleanings = await _cleaningService.GetRecentCleaningsAsync(limit);
        return Ok(cleanings);
    }

    /// <summary>
    /// Obter última limpeza (público)
    /// </summary>
    [HttpGet("latest")]
    public async Task<ActionResult<LastCleaningsResponse>> GetLatest()
    {
        var cleanings = await _cleaningService.GetLastCleaningsAsync();
        return Ok(cleanings);
    }

    /// <summary>
    /// Registar nova limpeza
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CleaningDto>> RecordCleaning([FromBody] RecordCleaningRequest request)
    {
        var cleaning = await _cleaningService.RecordCleaningAsync(request);
        return CreatedAtAction(nameof(GetCleaningById), new { id = cleaning.Id }, cleaning);
    }

    /// <summary>
    /// Obter limpeza por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<CleaningDto>> GetCleaningById(int id)
    {
        var cleaning = await _cleaningService.GetCleaningByIdAsync(id);
        if (cleaning == null) return NotFound();
        return Ok(cleaning);
    }

    /// <summary>
    /// Eliminar limpeza
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _cleaningService.DeleteCleaningAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}

