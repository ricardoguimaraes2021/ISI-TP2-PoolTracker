using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/workers")]
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
    [ProducesResponseType(typeof(List<WorkerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(typeof(WorkerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(ActiveWorkersResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ActiveWorkersResponse>> GetActive()
    {
        var response = await _workerService.GetActiveWorkersAsync();
        return Ok(response);
    }

    /// <summary>
    /// Obter informação do turno atual (público)
    /// </summary>
    /// <remarks>
    /// Retorna o tipo de turno atual (Manhã ou Tarde) baseado na hora local de Portugal.
    /// Turno da Manhã: 9:00 - 14:00
    /// Turno da Tarde: 14:00 - 19:00
    /// </remarks>
    [HttpGet("current-shift-info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult GetCurrentShiftInfo()
    {
        var shiftType = _workerService.DetermineCurrentShiftType();
        var isValidTime = _workerService.IsValidShiftTime();
        
        return Ok(new 
        { 
            currentShiftType = shiftType.ToString(),
            isWithinShiftHours = isValidTime,
            morningShiftHours = "09:00 - 14:00",
            afternoonShiftHours = "14:00 - 19:00"
        });
    }

    /// <summary>
    /// Criar novo trabalhador
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(WorkerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WorkerDto>> Create([FromBody] CreateWorkerRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { error = "Nome do trabalhador é obrigatório" });
        }
        
        try
        {
            var worker = await _workerService.CreateWorkerAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = worker.Id }, worker);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Erro ao criar trabalhador: {ex.Message}" });
        }
    }

    /// <summary>
    /// Atualizar trabalhador
    /// </summary>
    [HttpPut("{workerId}")]
    [Authorize]
    [ProducesResponseType(typeof(WorkerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string workerId)
    {
        var success = await _workerService.DeleteWorkerAsync(workerId);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Ativar turno de trabalhador
    /// </summary>
    /// <remarks>
    /// Marca um trabalhador como ativo em turno.
    /// O tipo de turno é determinado automaticamente baseado na hora atual:
    /// - Manhã (9:00 - 14:00): Turno da manhã
    /// - Tarde (14:00 - 19:00): Turno da tarde
    /// 
    /// Restrições:
    /// - Não é permitido iniciar turno com a piscina fechada
    /// - Não é permitido iniciar turno fora do horário de funcionamento (9:00 - 19:00)
    /// </remarks>
    [HttpPost("{workerId}/activate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ActivateShift(string workerId, [FromBody] ActivateShiftRequest? request = null)
    {
        var result = await _workerService.ActivateShiftAsync(workerId, request?.ShiftType);
        
        if (!result.Success)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }
        
        return Ok(new 
        { 
            message = "Turno iniciado com sucesso",
            shiftType = result.ShiftType?.ToString() ?? "Desconhecido"
        });
    }

    /// <summary>
    /// Desativar turno de trabalhador
    /// </summary>
    [HttpPost("{workerId}/deactivate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeactivateShift(string workerId)
    {
        var success = await _workerService.DeactivateShiftAsync(workerId);
        if (!success) return BadRequest(new { error = "Trabalhador não encontrado ou não está em turno" });
        return Ok(new { message = "Turno finalizado com sucesso" });
    }

    /// <summary>
    /// Obter estatísticas de turnos
    /// </summary>
    /// <remarks>
    /// Retorna estatísticas de turnos dos trabalhadores num período específico.
    /// Mostra contagem de turnos manhã, tarde e total para cada trabalhador.
    /// </remarks>
    [HttpGet("shift-stats")]
    [Authorize]
    [ProducesResponseType(typeof(List<ShiftStatsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ShiftStatsDto>>> GetShiftStats([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var end = endDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month));
        
        var stats = await _workerService.GetShiftStatsAsync(start, end);
        return Ok(stats);
    }

    /// <summary>
    /// Obter histórico de turnos de um trabalhador
    /// </summary>
    [HttpGet("{workerId}/shifts")]
    [Authorize]
    [ProducesResponseType(typeof(List<WorkerShiftDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<WorkerShiftDto>>> GetWorkerShifts(string workerId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var end = endDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month));
        
        var shifts = await _workerService.GetWorkerShiftsAsync(workerId, start, end);
        return Ok(shifts);
    }
}
