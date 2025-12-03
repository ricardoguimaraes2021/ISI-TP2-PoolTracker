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
    /// <remarks>
    /// Retorna o histórico de limpezas realizadas.
    /// Requer autenticação JWT.
    /// Por padrão, retorna as 10 limpezas mais recentes.
    /// </remarks>
    /// <param name="limit">Número máximo de registos a retornar (padrão: 10)</param>
    /// <returns>Lista de limpezas</returns>
    /// <response code="200">Histórico retornado com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<CleaningDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<CleaningDto>>> GetHistory([FromQuery] int limit = 10)
    {
        var cleanings = await _cleaningService.GetRecentCleaningsAsync(limit);
        return Ok(cleanings);
    }

    /// <summary>
    /// Obter última limpeza (público)
    /// </summary>
    /// <remarks>
    /// Retorna as últimas limpezas realizadas para balneários e WC.
    /// Endpoint público, não requer autenticação.
    /// Útil para exibição na página pública.
    /// </remarks>
    /// <returns>Últimas limpezas de balneários e WC</returns>
    /// <response code="200">Últimas limpezas retornadas com sucesso</response>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(LastCleaningsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LastCleaningsResponse>> GetLatest()
    {
        var cleanings = await _cleaningService.GetLastCleaningsAsync();
        return Ok(cleanings);
    }

    /// <summary>
    /// Registar nova limpeza
    /// </summary>
    /// <remarks>
    /// Regista uma nova limpeza realizada (balneários ou WC).
    /// Requer autenticação JWT.
    /// Permite adicionar notas opcionais sobre a limpeza.
    /// </remarks>
    /// <param name="request">Dados da limpeza (tipo: Balnearios ou Wc, notas opcionais)</param>
    /// <returns>Limpeza registada</returns>
    /// <response code="201">Limpeza registada com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CleaningDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CleaningDto>> RecordCleaning([FromBody] RecordCleaningRequest request)
    {
        var cleaning = await _cleaningService.RecordCleaningAsync(request);
        return CreatedAtAction(nameof(GetCleaningById), new { id = cleaning.Id }, cleaning);
    }

    /// <summary>
    /// Obter limpeza por ID
    /// </summary>
    /// <remarks>
    /// Retorna os detalhes de uma limpeza específica pelo seu ID.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="id">ID da limpeza</param>
    /// <returns>Detalhes da limpeza</returns>
    /// <response code="200">Limpeza encontrada</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Limpeza não encontrada</response>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(CleaningDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CleaningDto>> GetCleaningById(int id)
    {
        var cleaning = await _cleaningService.GetCleaningByIdAsync(id);
        if (cleaning == null) return NotFound();
        return Ok(cleaning);
    }

    /// <summary>
    /// Eliminar limpeza
    /// </summary>
    /// <remarks>
    /// Remove um registo de limpeza do sistema.
    /// Requer autenticação JWT.
    /// Use com cuidado, esta operação não pode ser desfeita.
    /// </remarks>
    /// <param name="id">ID da limpeza a eliminar</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Limpeza eliminada com sucesso</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Limpeza não encontrada</response>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _cleaningService.DeleteCleaningAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}

