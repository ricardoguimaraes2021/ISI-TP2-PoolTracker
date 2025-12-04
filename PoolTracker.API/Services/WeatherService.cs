using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using PoolTracker.Core.DTOs;

namespace PoolTracker.API.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    private const double DefaultLatitude = 41.5518;
    private const double DefaultLongitude = -8.4229;

    // Cache (configurável via appsettings)
    private WeatherInfoDto? _cachedWeather;
    private DateTime _cacheExpireTime = DateTime.MinValue;

    public WeatherService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("PoolTrackerApp/1.0");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
    }

    public async Task<WeatherInfoDto?> GetCurrentWeatherAsync()
    {
        var cacheMinutes = _configuration.GetValue<int>("OpenMeteo:CacheMinutes", 1);
        
        // Se o cache ainda é válido, devolve de imediato
        if (_cachedWeather != null && DateTime.UtcNow < _cacheExpireTime)
        {
            return _cachedWeather;
        }

        var lat = _configuration.GetValue<double>("OpenMeteo:Latitude", DefaultLatitude);
        var lon = _configuration.GetValue<double>("OpenMeteo:Longitude", DefaultLongitude);
        var baseUrl = _configuration.GetValue<string>("OpenMeteo:BaseUrl") ?? "https://api.open-meteo.com/v1/forecast";

        var url = $"{baseUrl}?latitude={lat.ToString(CultureInfo.InvariantCulture)}&longitude={lon.ToString(CultureInfo.InvariantCulture)}&current_weather=true&timezone=auto";

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Se falhar mas existe cache → devolve último valor
                if (_cachedWeather != null)
                {
                    return _cachedWeather;
                }
                return null;
            }

            var data = await response.Content.ReadFromJsonAsync<OpenMeteoResponse>();

            if (data?.CurrentWeather == null)
            {
                // devolve cache se existir
                if (_cachedWeather != null)
                    return _cachedWeather;
                return null;
            }

            var cw = data.CurrentWeather;

            // Guardar no cache
            _cachedWeather = new WeatherInfoDto
            {
                City = "Sobreposta, Braga",
                TemperatureC = cw.Temperature,
                WindSpeedKmh = cw.Windspeed,
                Description = MapWeatherCodeToDescription(cw.Weathercode),
                Icon = MapWeatherCodeToIcon(cw.Weathercode)
            };

            // Cache válido durante X minutos (configurável)
            _cacheExpireTime = DateTime.UtcNow.AddMinutes(cacheMinutes);

            return _cachedWeather;
        }
        catch
        {
            // Em caso de erro, devolve cache se existir
            return _cachedWeather;
        }
    }

    private static string MapWeatherCodeToDescription(int code) => code switch
    {
        0 => "Céu limpo",
        1 => "Maioritariamente limpo",
        2 => "Parcialmente nublado",
        3 => "Nublado",
        61 => "Chuva fraca",
        63 => "Chuva moderada",
        65 => "Chuva forte",
        80 => "Aguaceiros fracos",
        81 => "Aguaceiros moderados",
        82 => "Aguaceiros fortes",
        _ => "Condição desconhecida"
    };

    private static string MapWeatherCodeToIcon(int code) => code switch
    {
        0 => "sunny",
        1 or 2 => "cloudy",
        3 => "overcast",
        61 or 63 or 65 => "rain",
        80 or 81 or 82 => "showers",
        _ => "unknown"
    };

    private class OpenMeteoResponse
    {
        [JsonPropertyName("current_weather")]
        public CurrentWeather? CurrentWeather { get; set; }
    }

    private class CurrentWeather
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("windspeed")]
        public double Windspeed { get; set; }

        [JsonPropertyName("weathercode")]
        public int Weathercode { get; set; }
    }
}

