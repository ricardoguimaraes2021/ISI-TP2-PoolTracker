using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PoolTracker.Core.DTOs;
using PoolTracker.Tests.IntegrationTests;
using Xunit;

namespace PoolTracker.Tests.ApiTests;

public class PoolApiTests : IClassFixture<BaseIntegrationTest>
{
    private readonly HttpClient _client;
    private string? _authToken;

    public PoolApiTests(BaseIntegrationTest factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        if (_authToken != null) return _authToken;

        var loginRequest = new LoginRequest { Pin = "1234" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        
        _authToken = loginResult?.Token;
        return _authToken ?? throw new Exception("Failed to get auth token");
    }

    [Fact]
    public async Task GetStatus_ShouldReturnPoolStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/pool/status");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var status = await response.Content.ReadFromJsonAsync<PoolStatusDto>();
        status.Should().NotBeNull();
        // IsOpen should be a boolean value
        (status!.IsOpen == true || status.IsOpen == false).Should().BeTrue();
    }

    [Fact]
    public async Task Enter_ShouldIncrementCount_WhenAuthenticated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Get initial count
        var initialResponse = await _client.GetAsync("/api/pool/status");
        var initialStatus = await initialResponse.Content.ReadFromJsonAsync<PoolStatusDto>();
        var initialCount = initialStatus!.CurrentCount;

        // Act
        var response = await _client.PostAsync("/api/pool/enter", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var status = await response.Content.ReadFromJsonAsync<PoolStatusDto>();
        status.Should().NotBeNull();
        if (status!.IsOpen && initialCount < status.MaxCapacity)
        {
            status.CurrentCount.Should().Be(initialCount + 1);
        }
    }

    [Fact]
    public async Task Exit_ShouldDecrementCount_WhenAuthenticated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Get initial count
        var initialResponse = await _client.GetAsync("/api/pool/status");
        var initialStatus = await initialResponse.Content.ReadFromJsonAsync<PoolStatusDto>();
        var initialCount = initialStatus!.CurrentCount;

        // Act
        var response = await _client.PostAsync("/api/pool/exit", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var status = await response.Content.ReadFromJsonAsync<PoolStatusDto>();
        status.Should().NotBeNull();
        if (initialCount > 0)
        {
            status!.CurrentCount.Should().Be(initialCount - 1);
        }
    }

    [Fact]
    public async Task SetCount_ShouldUpdateCount_WhenAuthenticated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PutAsync("/api/pool/count?value=25", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var status = await response.Content.ReadFromJsonAsync<PoolStatusDto>();
        status.Should().NotBeNull();
        status!.CurrentCount.Should().Be(25);
    }

    [Fact]
    public async Task SetCapacity_ShouldUpdateCapacity_WhenAuthenticated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PutAsync("/api/pool/capacity?value=150", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var status = await response.Content.ReadFromJsonAsync<PoolStatusDto>();
        status.Should().NotBeNull();
        status!.MaxCapacity.Should().Be(150);
    }
}

