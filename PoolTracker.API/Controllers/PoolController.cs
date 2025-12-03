using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    [HttpGet("status")]
    public async Task<ActionResult<PoolStatusDto>> GetStatus()
    {
        var status = await _poolService.GetStatusAsync();
        return Ok(status);
    }

    /// <summary>
    /// Registar entrada de pessoa
    /// </summary>
    [HttpPost("enter")]
    [Authorize]
    public async Task<ActionResult<PoolStatusDto>> Enter()
    {
        var status = await _poolService.EnterAsync();
        return Ok(status);
    }

    /// <summary>
    /// Registar saída de pessoa
    /// </summary>
    [HttpPost("exit")]
    [Authorize]
    public async Task<ActionResult<PoolStatusDto>> Exit()
    {
        var status = await _poolService.ExitAsync();
        return Ok(status);
    }

    /// <summary>
    /// Definir contagem manualmente
    /// </summary>
    [HttpPut("count")]
    [Authorize]
    public async Task<ActionResult<PoolStatusDto>> SetCount([FromQuery] int value)
    {
        var status = await _poolService.SetCountAsync(value);
        return Ok(status);
    }

    /// <summary>
    /// Alterar capacidade máxima
    /// </summary>
    [HttpPut("capacity")]
    [Authorize]
    public async Task<ActionResult<PoolStatusDto>> SetCapacity([FromQuery] int value)
    {
        var status = await _poolService.SetCapacityAsync(value);
        return Ok(status);
    }

    /// <summary>
    /// Abrir/fechar piscina
    /// </summary>
    [HttpPut("open-status")]
    [Authorize]
    public async Task<ActionResult<PoolStatusDto>> SetOpenStatus([FromQuery] bool isOpen)
    {
        var status = await _poolService.SetOpenStatusAsync(isOpen);
        return Ok(status);
    }

    /// <summary>
    /// Resetar sistema (contagem e estado)
    /// </summary>
    [HttpDelete("reset")]
    [Authorize]
    public async Task<ActionResult<PoolStatusDto>> Reset()
    {
        var status = await _poolService.ResetAsync();
        return Ok(status);
    }
}

