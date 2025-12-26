using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;

namespace PoolTracker.API.Services;

public class ReportService : IReportService
{
    private readonly IRepository<DailyReport> _repository;
    private readonly PoolTrackerDbContext _context;
    private readonly IVisitService _visitService;
    private readonly IWaterQualityService _waterQualityService;
    private readonly IWorkerService _workerService;
    private readonly ICleaningService _cleaningService;

    public ReportService(
        IRepository<DailyReport> repository,
        PoolTrackerDbContext context,
        IVisitService visitService,
        IWaterQualityService waterQualityService,
        IWorkerService workerService,
        ICleaningService cleaningService)
    {
        _repository = repository;
        _context = context;
        _visitService = visitService;
        _waterQualityService = waterQualityService;
        _workerService = workerService;
        _cleaningService = cleaningService;
    }

    public async Task<DailyReportDto> GenerateDailyReportAsync(DateTime? closingTime = null)
    {
        var reportDate = DateOnly.FromDateTime(DateTime.UtcNow);
        closingTime ??= DateTime.UtcNow;

        // Obter dados do dia
        var totalVisitors = await _visitService.GetTodayVisitorsAsync();
        
        // Obter máxima ocupação (simplificado - pode ser melhorado com logs)
        var poolStatus = await _context.PoolStatus.FirstOrDefaultAsync();
        var maxOccupancy = poolStatus?.MaxCapacity ?? 120; // TODO: Calcular máximo real do dia
        
        // Obter última qualidade da água
        var waterQualityChildren = await _waterQualityService.GetLatestMeasurementAsync(PoolTracker.Core.Entities.PoolType.Criancas);
        var waterQualityAdults = await _waterQualityService.GetLatestMeasurementAsync(PoolTracker.Core.Entities.PoolType.Adultos);
        
        // Obter trabalhadores ativos
        var activeWorkersCount = await _workerService.GetActiveWorkersCountAsync();
        
        // Obter limpezas do dia
        var today = DateTime.UtcNow.Date;
        var todayStart = today;
        var todayEnd = today.AddDays(1).AddTicks(-1);
        var cleanings = await _cleaningService.GetRecentCleaningsAsync(50);
        // Filter in memory to avoid .Date translation issues with EF Core
        var todayCleanings = cleanings.Where(c => c.CleanedAt >= todayStart && c.CleanedAt <= todayEnd).ToList();

        // Verificar se já existe relatório para hoje
        var existingReport = await _context.DailyReports
            .FirstOrDefaultAsync(r => r.ReportDate == reportDate);

        var report = existingReport ?? new DailyReport
        {
            ReportDate = reportDate,
            ClosingTime = closingTime.Value
        };

        report.TotalVisitors = totalVisitors;
        report.MaxOccupancy = maxOccupancy;
        report.ClosingTime = closingTime.Value;
        report.WaterQualityChildren = waterQualityChildren != null 
            ? JsonSerializer.Serialize(new { waterQualityChildren.PhLevel, waterQualityChildren.Temperature, waterQualityChildren.MeasuredAt })
            : null;
        report.WaterQualityAdults = waterQualityAdults != null
            ? JsonSerializer.Serialize(new { waterQualityAdults.PhLevel, waterQualityAdults.Temperature, waterQualityAdults.MeasuredAt })
            : null;
        report.ActiveWorkersCount = JsonSerializer.Serialize(activeWorkersCount);
        report.CleaningRecords = JsonSerializer.Serialize(todayCleanings);

        if (existingReport == null)
        {
            await _repository.AddAsync(report);
        }
        else
        {
            await _repository.UpdateAsync(report);
        }

        return MapToDto(report);
    }

    public async Task<DailyReportDto?> GetReportByDateAsync(DateOnly date)
    {
        var report = await _context.DailyReports
            .FirstOrDefaultAsync(r => r.ReportDate == date);

        return report == null ? null : MapToDto(report);
    }

    public async Task<List<DailyReportDto>> GetReportsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        var reports = await _context.DailyReports
            .Where(r => r.ReportDate >= startDate && r.ReportDate <= endDate)
            .OrderByDescending(r => r.ReportDate)
            .ToListAsync();

        return reports.Select(MapToDto).ToList();
    }

    public async Task<DailyReportDto?> GetLatestReportAsync()
    {
        var report = await _context.DailyReports
            .OrderByDescending(r => r.ReportDate)
            .FirstOrDefaultAsync();

        return report == null ? null : MapToDto(report);
    }

    private DailyReportDto MapToDto(DailyReport report)
    {
        return new DailyReportDto
        {
            Id = report.Id,
            ReportDate = report.ReportDate,
            TotalVisitors = report.TotalVisitors,
            MaxOccupancy = report.MaxOccupancy,
            AvgOccupancy = report.AvgOccupancy,
            OpeningTime = report.OpeningTime,
            ClosingTime = report.ClosingTime,
            WaterQualityChildren = report.WaterQualityChildren != null 
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(report.WaterQualityChildren) 
                : null,
            WaterQualityAdults = report.WaterQualityAdults != null
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(report.WaterQualityAdults)
                : null,
            ActiveWorkersCount = report.ActiveWorkersCount != null
                ? JsonSerializer.Deserialize<Dictionary<string, int>>(report.ActiveWorkersCount)
                : null,
            CleaningRecords = report.CleaningRecords != null
                ? JsonSerializer.Deserialize<List<Dictionary<string, object>>>(report.CleaningRecords)
                : null,
            CreatedAt = report.CreatedAt
        };
    }
}

