namespace PoolTracker.Core.DTOs;

public class LoginRequest
{
    public string Pin { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

