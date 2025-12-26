using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using PoolTracker.Core.DTOs;
using Xunit;

namespace PoolTracker.Tests.IntegrationTests.Controllers;

public class PoolControllerTests : IClassFixture<BaseIntegrationTest>
{
    private readonly HttpClient _client;

    public PoolControllerTests(BaseIntegrationTest factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetStatus_ShouldReturnOk_WhenCalled()
    {
        // Act
        var response = await _client.GetAsync("/api/pool/status");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var status = await response.Content.ReadFromJsonAsync<PoolStatusDto>();
        status.Should().NotBeNull();
        status!.CurrentCount.Should().BeGreaterThanOrEqualTo(0);
        status.MaxCapacity.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Enter_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await _client.PostAsync("/api/pool/enter", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Exit_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await _client.PostAsync("/api/pool/exit", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SetCount_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await _client.PutAsync("/api/pool/count?value=10", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

