using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PoolTracker.Infrastructure.Data;
using PoolTracker.Infrastructure.Repositories;
using PoolTracker.Core.Interfaces;
using PoolTracker.Core.Entities;
using System.Text;

namespace PoolTracker.API;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();

        // Database
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=localhost;Database=PoolTrackerDB;Trusted_Connection=true;TrustServerCertificate=true;";

        builder.Services.AddDbContext<PoolTrackerDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // HttpClient for WeatherService
        builder.Services.AddHttpClient<Services.IWeatherService, Services.WeatherService>();

        // Services
        builder.Services.AddScoped<Services.IPoolService, Services.PoolService>();
        builder.Services.AddScoped<Services.IVisitService, Services.VisitService>();
        builder.Services.AddScoped<Services.IWorkerService, Services.WorkerService>();
        builder.Services.AddScoped<Services.IWaterQualityService, Services.WaterQualityService>();
        builder.Services.AddScoped<Services.ICleaningService, Services.CleaningService>();
        builder.Services.AddScoped<Services.IReportService, Services.ReportService>();
        builder.Services.AddScoped<Services.IStatisticsService, Services.StatisticsService>();
        builder.Services.AddScoped<Services.IShoppingService, Services.ShoppingService>();
        builder.Services.AddScoped<Services.IJwtService, Services.JwtService>();

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // JWT Authentication
        var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
        var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "PoolTrackerAPI";
        var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "PoolTrackerClients";

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization();

        // Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(Configuration.SwaggerConfiguration.ConfigureSwagger);


        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "PoolTracker API v1");
                options.RoutePrefix = "swagger";
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Ensure database is created
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<PoolTrackerDbContext>();
            dbContext.Database.EnsureCreated();
            
            // Seed initial data if needed
            if (!dbContext.PoolStatus.Any())
            {
                dbContext.PoolStatus.Add(new PoolStatus
                {
                    CurrentCount = 0,
                    MaxCapacity = 120,
                    IsOpen = true,
                    LocationName = "Piscina Municipal da Sobreposta",
                    Address = "R. da Piscina 22, 4715-553 Sobreposta",
                    Phone = "253 636 948"
                });
                dbContext.SaveChanges();
            }
        }

        app.Run();
    }
}
