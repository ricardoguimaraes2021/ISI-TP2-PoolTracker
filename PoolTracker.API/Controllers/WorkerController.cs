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
    /// <remarks>
    /// Retorna uma lista de todos os trabalhadores cadastrados.
    /// Requer autenticação JWT.
    /// Pode filtrar apenas trabalhadores ativos usando o parâmetro activeOnly.
    /// </remarks>
    /// <param name="activeOnly">Se true, retorna apenas trabalhadores ativos (padrão: false)</param>
    /// <returns>Lista de trabalhadores</returns>
    /// <response code="200">Lista de trabalhadores retornada com sucesso</response>
    /// <response code="401">Não autenticado</response>
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
    /// <remarks>
    /// Retorna os detalhes de um trabalhador específico pelo seu ID numérico.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="id">ID numérico do trabalhador</param>
    /// <returns>Detalhes do trabalhador</returns>
    /// <response code="200">Trabalhador encontrado</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Trabalhador não encontrado</response>
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
    /// <remarks>
    /// Retorna a lista de trabalhadores que estão atualmente em turno.
    /// Endpoint público, não requer autenticação.
    /// Inclui contagem de trabalhadores por função e lista detalhada.
    /// </remarks>
    /// <returns>Lista de trabalhadores ativos com contagens por função</returns>
    /// <response code="200">Lista de trabalhadores ativos retornada com sucesso</response>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ActiveWorkersResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ActiveWorkersResponse>> GetActive()
    {
        var response = await _workerService.GetActiveWorkersAsync();
        return Ok(response);
    }

    /// <summary>
    /// Criar novo trabalhador
    /// </summary>
    /// <remarks>
    /// Cria um novo trabalhador no sistema.
    /// Requer autenticação JWT.
    /// Se o WorkerId não for fornecido, será gerado automaticamente (formato W0001, W0002, etc.).
    /// </remarks>
    /// <param name="request">Dados do trabalhador a criar</param>
    /// <returns>Trabalhador criado</returns>
    /// <response code="201">Trabalhador criado com sucesso</response>
    /// <response code="401">Não autenticado</response>
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
    /// <remarks>
    /// Atualiza os dados de um trabalhador existente.
    /// Requer autenticação JWT.
    /// Apenas os campos fornecidos no request serão atualizados.
    /// </remarks>
    /// <param name="workerId">ID único do trabalhador (formato W0001)</param>
    /// <param name="request">Dados a atualizar (todos os campos são opcionais)</param>
    /// <returns>Trabalhador atualizado</returns>
    /// <response code="200">Trabalhador atualizado com sucesso</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Trabalhador não encontrado</response>
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
    /// <remarks>
    /// Remove um trabalhador do sistema (soft delete - marca como inativo).
    /// Requer autenticação JWT.
    /// O trabalhador não será eliminado fisicamente, apenas marcado como inativo.
    /// </remarks>
    /// <param name="workerId">ID único do trabalhador (formato W0001)</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Trabalhador eliminado com sucesso</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Trabalhador não encontrado</response>
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
    /// Requer autenticação JWT.
    /// O tipo de turno pode ser "Manha" (9h-14h) ou "Tarde" (14h-19h).
    /// Se o trabalhador já estiver em turno, a operação é ignorada.
    /// </remarks>
    /// <param name="workerId">ID único do trabalhador (formato W0001)</param>
    /// <param name="request">Tipo de turno a ativar</param>
    /// <returns>Confirmação de ativação</returns>
    /// <response code="200">Turno ativado com sucesso</response>
    /// <response code="400">Trabalhador não encontrado ou já está em turno</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost("{workerId}/activate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ActivateShift(string workerId, [FromBody] ActivateShiftRequest request)
    {
        var success = await _workerService.ActivateShiftAsync(workerId, request.ShiftType);
        if (!success) return BadRequest("Worker not found or already on shift");
        return Ok(new { message = "Shift activated" });
    }

    /// <summary>
    /// Desativar turno de trabalhador
    /// </summary>
    /// <remarks>
    /// Marca o fim do turno de um trabalhador.
    /// Requer autenticação JWT.
    /// Registra o horário de término do turno.
    /// </remarks>
    /// <param name="workerId">ID único do trabalhador (formato W0001)</param>
    /// <returns>Confirmação de desativação</returns>
    /// <response code="200">Turno desativado com sucesso</response>
    /// <response code="400">Trabalhador não encontrado ou não está em turno</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost("{workerId}/deactivate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeactivateShift(string workerId)
    {
        var success = await _workerService.DeactivateShiftAsync(workerId);
        if (!success) return BadRequest("Worker not found or not on shift");
        return Ok(new { message = "Shift deactivated" });
    }

    /// <summary>
    /// Obter estatísticas de turnos
    /// </summary>
    /// <remarks>
    /// Retorna estatísticas de turnos dos trabalhadores num período específico.
    /// Requer autenticação JWT.
    /// Agrupa por trabalhador e mostra contagem de turnos manhã e tarde.
    /// </remarks>
    /// <param name="startDate">Data de início (formato: YYYY-MM-DD)</param>
    /// <param name="endDate">Data de fim (formato: YYYY-MM-DD)</param>
    /// <returns>Estatísticas de turnos</returns>
    /// <response code="200">Estatísticas retornadas com sucesso</response>
    /// <response code="401">Não autenticado</response>
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
    /// <remarks>
    /// Retorna o histórico de turnos de um trabalhador específico num período.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="workerId">ID único do trabalhador (formato W0001)</param>
    /// <param name="startDate">Data de início (formato: YYYY-MM-DD)</param>
    /// <param name="endDate">Data de fim (formato: YYYY-MM-DD)</param>
    /// <returns>Histórico de turnos</returns>
    /// <response code="200">Histórico retornado com sucesso</response>
    /// <response code="401">Não autenticado</response>
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

