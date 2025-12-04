using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;

    public AuthController(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    /// <summary>
    /// Login com PIN (gera JWT token)
    /// </summary>
    /// <remarks>
    /// Autentica um utilizador usando um PIN e retorna um token JWT.
    /// Endpoint público, não requer autenticação.
    /// O PIN padrão é configurável em appsettings.json (AdminPin).
    /// O token JWT expira após o tempo configurado (padrão: 60 minutos).
    /// </remarks>
    /// <param name="request">PIN de autenticação</param>
    /// <returns>Token JWT e refresh token</returns>
    /// <response code="200">Login bem-sucedido, token JWT retornado</response>
    /// <response code="401">PIN inválido</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _jwtService.GenerateTokenAsync(request.Pin);
        
        if (response == null)
        {
            return Unauthorized(new { error = "PIN inválido" });
        }

        return Ok(response);
    }

    /// <summary>
    /// Atualizar token JWT (refresh token)
    /// </summary>
    /// <remarks>
    /// Gera um novo token JWT usando um refresh token válido.
    /// Requer autenticação JWT.
    /// Útil para manter a sessão ativa sem precisar fazer login novamente.
    /// </remarks>
    /// <param name="request">Refresh token</param>
    /// <returns>Novo token JWT e refresh token</returns>
    /// <response code="200">Token atualizado com sucesso</response>
    /// <response code="401">Refresh token inválido ou expirado</response>
    [HttpPost("refresh")]
    [Authorize]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var response = await _jwtService.RefreshTokenAsync(request.RefreshToken);
        
        if (response == null)
        {
            return Unauthorized(new { error = "Refresh token inválido" });
        }

        return Ok(response);
    }
}

