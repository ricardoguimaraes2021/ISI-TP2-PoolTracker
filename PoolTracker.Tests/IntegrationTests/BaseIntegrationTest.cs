using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PoolTracker.API;
using PoolTracker.Infrastructure.Data;

namespace PoolTracker.Tests.IntegrationTests;

public class BaseIntegrationTest : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
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

            // Garante que a base de dados é criada
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<PoolTrackerDbContext>();
                db.Database.EnsureCreated();
            }
        });
    }
}

