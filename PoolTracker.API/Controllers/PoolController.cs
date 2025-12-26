using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/pool")]
public class PoolController : ControllerBase
{
    private readonly IPoolService _poolService;

    public PoolController(IPoolService poolService)
    {
        _poolService = poolService;
    }

    /// <summary>
    /// Obter estado atual da piscina
    /// </summary>
    /// <remarks>
    /// Retorna o estado atual da piscina incluindo:
    /// - Contagem atual de pessoas
    /// - Capacidade máxima
    /// - Estado (aberta/fechada)
    /// - Informações de localização
    /// - Horário de funcionamento do dia
    /// </remarks>
    /// <returns>Estado atual da piscina</returns>
    /// <response code="200">Retorna o estado da piscina com sucesso</response>
    [HttpGet("status")]
    [ProducesResponseType(typeof(PoolStatusDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PoolStatusDto>> GetStatus()
    {
        var status = await _poolService.GetStatusAsync();
        return Ok(status);
    }

    /// <summary>
    /// Registar entrada de pessoa na piscina
    /// </summary>
    /// <remarks>
    /// Incrementa o contador de pessoas na piscina.
    /// Requer autenticação JWT.
    /// Não permite entrada se a piscina estiver fechada ou com lotação máxima atingida.
    /// </remarks>
    /// <returns>Estado atualizado da piscina</returns>
    /// <response code="200">Entrada registada com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost("enter")]
    [Authorize]
    [ProducesResponseType(typeof(PoolStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PoolStatusDto>> Enter()
    {
        var status = await _poolService.EnterAsync();
        return Ok(status);
    }

    /// <summary>
    /// Registar saída de pessoa da piscina
    /// </summary>
    /// <remarks>
    /// Decrementa o contador de pessoas na piscina.
    /// Requer autenticação JWT.
    /// O contador nunca fica negativo.
    /// </remarks>
    /// <returns>Estado atualizado da piscina</returns>
    /// <response code="200">Saída registada com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost("exit")]
    [Authorize]
    [ProducesResponseType(typeof(PoolStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PoolStatusDto>> Exit()
    {
        var status = await _poolService.ExitAsync();
        return Ok(status);
    }

    /// <summary>
    /// Definir contagem manualmente
    /// </summary>
    /// <remarks>
    /// Permite definir o número de pessoas na piscina manualmente.
    /// Requer autenticação JWT.
    /// O valor será ajustado automaticamente se exceder a capacidade máxima ou for negativo.
    /// </remarks>
    /// <param name="value">Número de pessoas (0 a capacidade máxima)</param>
    /// <returns>Estado atualizado da piscina</returns>
    /// <response code="200">Contagem atualizada com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpPut("count")]
    [Authorize]
    [ProducesResponseType(typeof(PoolStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PoolStatusDto>> SetCount([FromQuery] int value)
    {
        var status = await _poolService.SetCountAsync(value);
        return Ok(status);
    }

    /// <summary>
    /// Alterar capacidade máxima da piscina
    /// </summary>
    /// <remarks>
    /// Define a capacidade máxima de pessoas permitidas na piscina.
    /// Requer autenticação JWT.
    /// Se a contagem atual exceder a nova capacidade, será ajustada automaticamente.
    /// </remarks>
    /// <param name="value">Nova capacidade máxima (mínimo 1)</param>
    /// <returns>Estado atualizado da piscina</returns>
    /// <response code="200">Capacidade atualizada com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpPut("capacity")]
    [Authorize]
    [ProducesResponseType(typeof(PoolStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PoolStatusDto>> SetCapacity([FromQuery] int value)
    {
        var status = await _poolService.SetCapacityAsync(value);
        return Ok(status);
    }

    /// <summary>
    /// Abrir ou fechar a piscina
    /// </summary>
    /// <remarks>
    /// Permite abrir ou fechar a piscina.
    /// Requer autenticação JWT.
    /// Ao fechar a piscina, todos os trabalhadores ativos são desativados automaticamente e um relatório diário é gerado.
    /// </remarks>
    /// <param name="isOpen">true para abrir, false para fechar</param>
    /// <returns>Estado atualizado da piscina</returns>
    /// <response code="200">Estado atualizado com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpPut("open-status")]
    [Authorize]
    [ProducesResponseType(typeof(PoolStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PoolStatusDto>> SetOpenStatus([FromQuery] bool isOpen)
    {
        var status = await _poolService.SetOpenStatusAsync(isOpen);
        return Ok(status);
    }

    /// <summary>
    /// Resetar sistema (contagem e estado)
    /// </summary>
    /// <remarks>
    /// Reseta a contagem de pessoas para 0 e fecha a piscina.
    /// Requer autenticação JWT.
    /// Use com cuidado, esta operação não pode ser desfeita.
    /// </remarks>
    /// <returns>Estado resetado da piscina</returns>
    /// <response code="200">Sistema resetado com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpDelete("reset")]
    [Authorize]
    [ProducesResponseType(typeof(PoolStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PoolStatusDto>> Reset()
    {
        var status = await _poolService.ResetAsync();
        return Ok(status);
    }
}

