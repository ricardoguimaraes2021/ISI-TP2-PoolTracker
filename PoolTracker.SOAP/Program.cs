using Microsoft.EntityFrameworkCore;
using PoolTracker.Infrastructure.Data;
using PoolTracker.Infrastructure.Repositories;
using PoolTracker.Core.Interfaces;
using PoolTracker.SOAP.Contracts;
using PoolTracker.SOAP.Services;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=PoolTrackerDB;Trusted_Connection=true;TrustServerCertificate=true;";

builder.Services.AddDbContext<PoolTrackerDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// SOAP Services
builder.Services.AddScoped<IPoolDataService, PoolDataService>();
builder.Services.AddScoped<IWorkerDataService, WorkerDataService>();
builder.Services.AddScoped<IWaterQualityDataService, WaterQualityDataService>();
builder.Services.AddScoped<IReportDataService, ReportDataService>();

var app = builder.Build();

// Configure SOAP endpoints
IApplicationBuilder appBuilder = app;
appBuilder.UseSoapEndpoint<IPoolDataService>("/soap/PoolDataService", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
appBuilder.UseSoapEndpoint<IWorkerDataService>("/soap/WorkerDataService", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
appBuilder.UseSoapEndpoint<IWaterQualityDataService>("/soap/WaterQualityDataService", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
appBuilder.UseSoapEndpoint<IReportDataService>("/soap/ReportDataService", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PoolTrackerDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
