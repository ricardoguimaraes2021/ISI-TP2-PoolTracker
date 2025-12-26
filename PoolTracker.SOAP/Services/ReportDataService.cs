using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;
using PoolTracker.SOAP.Contracts;
using PoolTracker.SOAP.DataContracts;

namespace PoolTracker.SOAP.Services;

public class ReportDataService : IReportDataService
{
    private readonly IRepository<Core.Entities.DailyReport> _repository;
    private readonly PoolTrackerDbContext _context;

    public ReportDataService(IRepository<Core.Entities.DailyReport> repository, PoolTrackerDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public List<DailyReportData> GetReports(DateTime startDate, DateTime endDate)
    {
        var start = DateOnly.FromDateTime(startDate);
        var end = DateOnly.FromDateTime(endDate);

        var reports = _context.DailyReports
            .Where(r => r.ReportDate >= start && r.ReportDate <= end)
            .OrderByDescending(r => r.ReportDate)
            .ToList();

        return reports.Select(r => new DailyReportData
        {
            Id = r.Id,
            ReportDate = r.ReportDate.ToDateTime(TimeOnly.MinValue),
            TotalVisitors = r.TotalVisitors,
            MaxOccupancy = r.MaxOccupancy,
            AvgOccupancy = r.AvgOccupancy,
            OpeningTime = r.OpeningTime,
            ClosingTime = r.ClosingTime,
            WaterQualityChildren = r.WaterQualityChildren,
            WaterQualityAdults = r.WaterQualityAdults,
            ActiveWorkersCount = r.ActiveWorkersCount,
            CleaningRecords = r.CleaningRecords,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public DailyReportData GenerateReport(DateTime date)
    {
        var reportDate = DateOnly.FromDateTime(date);
        var report = _context.DailyReports
            .FirstOrDefault(r => r.ReportDate == reportDate);

        if (report == null)
        {
            // Criar relatório básico se não existir
            report = new Core.Entities.DailyReport
            {
                ReportDate = reportDate,
                TotalVisitors = 0,
                MaxOccupancy = 0,
                ClosingTime = date
            };
            _repository.AddAsync(report).Wait();
        }

        return new DailyReportData
        {
            Id = report.Id,
            ReportDate = report.ReportDate.ToDateTime(TimeOnly.MinValue),
            TotalVisitors = report.TotalVisitors,
            MaxOccupancy = report.MaxOccupancy,
            AvgOccupancy = report.AvgOccupancy,
            OpeningTime = report.OpeningTime,
            ClosingTime = report.ClosingTime,
            WaterQualityChildren = report.WaterQualityChildren,
            WaterQualityAdults = report.WaterQualityAdults,
            ActiveWorkersCount = report.ActiveWorkersCount,
            CleaningRecords = report.CleaningRecords,
            CreatedAt = report.CreatedAt
        };
    }
}

