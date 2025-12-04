using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PoolTracker.API;
using PoolTracker.Core.Entities;
using PoolTracker.Infrastructure.Data;

namespace PoolTracker.Tests.IntegrationTests;

public class BaseIntegrationTest : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Configurações para testes
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TestSecretKey12345678901234567890",
                ["Jwt:Issuer"] = "PoolTracker.Tests",
                ["Jwt:Audience"] = "PoolTracker.Tests",
                ["AdminPin"] = "1234"
            });
        });
        
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext real
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<PoolTrackerDbContext>));
            
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Adiciona DbContext em memória para testes
            services.AddDbContext<PoolTrackerDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
            });

            // Garante que a base de dados é criada e inicializada com dados de teste
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<PoolTrackerDbContext>();
                db.Database.EnsureCreated();
                
                // Seed data para testes
                SeedTestData(db);
            }
        });
    }

    private static void SeedTestData(PoolTrackerDbContext db)
    {
        // Inicializar PoolStatus
        if (!db.PoolStatus.Any())
        {
            db.PoolStatus.Add(new PoolStatus
            {
                CurrentCount = 0,
                MaxCapacity = 120,
                IsOpen = true,
                LocationName = "Piscina Municipal (Test)",
                Address = "Rua de Teste, 123",
                Phone = "123456789",
                LastUpdated = DateTime.UtcNow
            });
        }

        // Adicionar alguns trabalhadores de teste
        if (!db.Workers.Any())
        {
            db.Workers.AddRange(new[]
            {
                new Worker
                {
                    WorkerId = "W0001",
                    Name = "João Teste",
                    Role = WorkerRole.NadadorSalvador,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Worker
                {
                    WorkerId = "W0002",
                    Name = "Maria Teste",
                    Role = WorkerRole.Bar,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            });
        }

        db.SaveChanges();
    }
}

