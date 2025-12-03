using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkerController : ControllerBase
{
    private readonly IWorkerService _workerService;

    public WorkerController(IWorkerService workerService)
    {
        _workerService = workerService;
    }

    /// <summary>
    /// Listar todos os trabalhadores
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<WorkerDto>>> GetAll([FromQuery] bool activeOnly = false)
    {
        var workers = await _workerService.GetAllWorkersAsync(activeOnly);
        return Ok(workers);
    }

    /// <summary>
    /// Obter trabalhador por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<WorkerDto>> GetById(int id)
    {
        var worker = await _workerService.GetWorkerByIdAsync(id);
        if (worker == null) return NotFound();
        return Ok(worker);
    }

    /// <summary>
    /// Listar trabalhadores ativos (público)
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<ActiveWorkersResponse>> GetActive()
    {
        var response = await _workerService.GetActiveWorkersAsync();
        return Ok(response);
    }

    /// <summary>
    /// Criar novo trabalhador
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<WorkerDto>> Create([FromBody] CreateWorkerRequest request)
    {
        var worker = await _workerService.CreateWorkerAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = worker.Id }, worker);
    }

    /// <summary>
    /// Atualizar trabalhador
    /// </summary>
    [HttpPut("{workerId}")]
    [Authorize]
    public async Task<ActionResult<WorkerDto>> Update(string workerId, [FromBody] UpdateWorkerRequest request)
    {
        var worker = await _workerService.UpdateWorkerAsync(workerId, request);
        if (worker == null) return NotFound();
        return Ok(worker);
    }

    /// <summary>
    /// Eliminar trabalhador
    /// </summary>
    [HttpDelete("{workerId}")]
    [Authorize]
    public async Task<ActionResult> Delete(string workerId)
    {
        var success = await _workerService.DeleteWorkerAsync(workerId);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Ativar turno de trabalhador
    /// </summary>
    [HttpPost("{workerId}/activate")]
    [Authorize]
    public async Task<ActionResult> ActivateShift(string workerId, [FromBody] ActivateShiftRequest request)
    {
        var success = await _workerService.ActivateShiftAsync(workerId, request.ShiftType);
        if (!success) return BadRequest("Worker not found or already on shift");
        return Ok(new { message = "Shift activated" });
    }

    /// <summary>
    /// Desativar turno de trabalhador
    /// </summary>
    [HttpPost("{workerId}/deactivate")]
    [Authorize]
    public async Task<ActionResult> DeactivateShift(string workerId)
    {
        var success = await _workerService.DeactivateShiftAsync(workerId);
        if (!success) return BadRequest("Worker not found or not on shift");
        return Ok(new { message = "Shift deactivated" });
    }
}

