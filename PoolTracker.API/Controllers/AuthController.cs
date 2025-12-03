using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoolTracker.API.Services;
using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    [HttpPost("login")]
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
    /// Refresh token
    /// </summary>
    [HttpPost("refresh")]
    [Authorize]
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

