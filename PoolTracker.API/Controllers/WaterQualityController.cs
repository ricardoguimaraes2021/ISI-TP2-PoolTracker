using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/water-quality")]
public class WaterQualityController : ControllerBase
{
    private readonly IWaterQualityService _waterQualityService;

    public WaterQualityController(IWaterQualityService waterQualityService)
    {
        _waterQualityService = waterQualityService;
    }

    /// <summary>
    /// Obter histórico de medições de qualidade da água
    /// </summary>
    /// <remarks>
    /// Retorna o histórico de medições de qualidade da água.
    /// Endpoint público, não requer autenticação.
    /// Por padrão, retorna medições dos últimos 30 dias.
    /// Pode filtrar por tipo de piscina (Criancas ou Adultos).
    /// </remarks>
    /// <param name="startDate">Data de início (padrão: 30 dias atrás)</param>
    /// <param name="endDate">Data de fim (padrão: hoje)</param>
    /// <param name="poolType">Tipo de piscina (Criancas ou Adultos) - opcional</param>
    /// <returns>Lista de medições de qualidade da água</returns>
    /// <response code="200">Histórico retornado com sucesso</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<WaterQualityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WaterQualityDto>>> GetHistory(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? poolType = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;
        
        PoolType? poolTypeEnum = null;
        if (!string.IsNullOrEmpty(poolType) && Enum.TryParse<PoolType>(poolType, true, out var parsedPoolType))
        {
            poolTypeEnum = parsedPoolType;
        }
        
        var measurements = await _waterQualityService.GetMeasurementsByDateRangeAsync(start, end, poolTypeEnum);
        return Ok(measurements);
    }

    /// <summary>
    /// Obter última medição de qualidade da água (público)
    /// </summary>
    /// <remarks>
    /// Retorna as últimas medições de qualidade da água.
    /// Endpoint público, não requer autenticação.
    /// Se poolType for fornecido, retorna apenas a medição dessa piscina.
    /// Se não for fornecido, retorna medições de ambas as piscinas (crianças e adultos).
    /// </remarks>
    /// <param name="poolType">Tipo de piscina (Criancas ou Adultos) - opcional</param>
    /// <returns>Última(s) medição(ões) de qualidade da água</returns>
    /// <response code="200">Última(s) medição(ões) retornada(s) com sucesso</response>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(CurrentMeasurementsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(WaterQualityDto), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetLatest([FromQuery] PoolType? poolType = null)
    {
        if (poolType.HasValue)
        {
            var measurement = await _waterQualityService.GetLatestMeasurementAsync(poolType.Value);
            if (measurement == null) return NotFound();
            return Ok(measurement);
        }
        
        var measurements = await _waterQualityService.GetCurrentMeasurementsAsync();
        return Ok(measurements);
    }

    /// <summary>
    /// Registar nova medição de qualidade da água
    /// </summary>
    /// <remarks>
    /// Regista uma nova medição de qualidade da água para uma piscina.
    /// Requer autenticação JWT.
    /// A medição inclui nível de pH e temperatura.
    /// </remarks>
    /// <param name="request">Dados da medição (tipo de piscina, pH, temperatura, notas opcionais)</param>
    /// <returns>Medição registada</returns>
    /// <response code="201">Medição registada com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(WaterQualityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WaterQualityDto>> RecordMeasurement([FromBody] RecordMeasurementRequest request)
    {
        var measurement = await _waterQualityService.RecordMeasurementAsync(request);
        return CreatedAtAction(nameof(GetMeasurementById), new { id = measurement.Id }, measurement);
    }

    /// <summary>
    /// Obter medição de qualidade da água por ID
    /// </summary>
    /// <remarks>
    /// Retorna os detalhes de uma medição específica pelo seu ID.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="id">ID da medição</param>
    /// <returns>Detalhes da medição</returns>
    /// <response code="200">Medição encontrada</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Medição não encontrada</response>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(WaterQualityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WaterQualityDto>> GetMeasurementById(int id)
    {
        var measurement = await _waterQualityService.GetMeasurementByIdAsync(id);
        if (measurement == null) return NotFound();
        return Ok(measurement);
    }

    /// <summary>
    /// Eliminar medição de qualidade da água
    /// </summary>
    /// <remarks>
    /// Remove uma medição de qualidade da água do sistema.
    /// Requer autenticação JWT.
    /// Use com cuidado, esta operação não pode ser desfeita.
    /// </remarks>
    /// <param name="id">ID da medição a eliminar</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Medição eliminada com sucesso</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Medição não encontrada</response>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _waterQualityService.DeleteMeasurementAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}

