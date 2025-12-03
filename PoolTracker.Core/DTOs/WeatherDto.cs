namespace PoolTracker.Core.DTOs;

public class WeatherInfoDto
{
    public string City { get; set; } = string.Empty;
    public double TemperatureC { get; set; }
    public double WindSpeedKmh { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

