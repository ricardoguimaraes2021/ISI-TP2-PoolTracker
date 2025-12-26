using System;
using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;

namespace PoolTracker.API.Services;

public class PoolService : IPoolService
{
    private readonly IRepository<PoolStatus> _repository;
    private readonly PoolTrackerDbContext _context;
    private readonly IVisitService _visitService;
    private readonly IWorkerService? _workerService;
    private readonly IReportService? _reportService;

    private static readonly Dictionary<int, string> OpeningHours = new()
    {
        { 0, "09:00–19:00" }, // Sunday
        { 1, "10:00–19:00" }, // Monday
        { 2, "10:00–19:00" }, // Tuesday
        { 3, "10:00–19:00" }, // Wednesday
        { 4, "10:00–19:00" }, // Thursday
        { 5, "10:00–19:00" }, // Friday
        { 6, "09:00–19:00" }  // Saturday
    };

    public PoolService(
        IRepository<PoolStatus> repository, 
        PoolTrackerDbContext context, 
        IVisitService visitService,
        IWorkerService? workerService = null,
        IReportService? reportService = null)
    {
        _repository = repository;
        _context = context;
        _visitService = visitService;
        _workerService = workerService;
        _reportService = reportService;
    }

    private string GetTodayOpeningHours()
    {
        var dayOfWeek = (int)DateTime.UtcNow.DayOfWeek;
        return OpeningHours.GetValueOrDefault(dayOfWeek, "Encerrado");
    }

    public async Task<PoolStatusDto> GetStatusAsync()
    {
        var status = await _context.PoolStatus.FirstOrDefaultAsync();
        
        if (status == null)
        {
            // Criar estado inicial se não existir
            status = new PoolStatus
            {
                CurrentCount = 0,
                MaxCapacity = 120,
                IsOpen = true,
                LocationName = "Piscina Municipal da Sobreposta",
                Address = "R. da Piscina 22, 4715-553 Sobreposta",
                Phone = "253 636 948"
            };
            await _repository.AddAsync(status);
        }

        return new PoolStatusDto
        {
            CurrentCount = status.CurrentCount,
            MaxCapacity = status.MaxCapacity,
            IsOpen = status.IsOpen,
            LastUpdated = status.LastUpdated,
            LocationName = status.LocationName,
            Address = status.Address,
            Phone = status.Phone,
            TodayOpeningHours = GetTodayOpeningHours()
        };
    }

    public async Task<PoolStatusDto> EnterAsync()
    {
        var status = await _context.PoolStatus.FirstOrDefaultAsync();
        if (status == null) return await GetStatusAsync();

        // Não permite entrada se estiver fechada
        if (!status.IsOpen)
        {
            return await GetStatusAsync();
        }

        // Não permite ultrapassar a capacidade
        if (status.CurrentCount < status.MaxCapacity)
        {
            status.CurrentCount++;
            status.LastUpdated = DateTime.UtcNow;
            await _repository.UpdateAsync(status);

            // Registrar visitante diário
            await _visitService.IncrementDailyVisitorsAsync();
        }

        return await GetStatusAsync();
    }

    public async Task<PoolStatusDto> ExitAsync()
    {
        var status = await _context.PoolStatus.FirstOrDefaultAsync();
        if (status == null) return await GetStatusAsync();

        if (status.CurrentCount > 0)
        {
            status.CurrentCount--;
            status.LastUpdated = DateTime.UtcNow;
            await _repository.UpdateAsync(status);
        }

        return await GetStatusAsync();
    }

    public async Task<PoolStatusDto> SetCountAsync(int value)
    {
        var status = await _context.PoolStatus.FirstOrDefaultAsync();
        if (status == null) return await GetStatusAsync();

        // Clamp entre 0 e maxCapacity
        value = Math.Max(0, Math.Min(value, status.MaxCapacity));
        status.CurrentCount = value;
        status.LastUpdated = DateTime.UtcNow;
        await _repository.UpdateAsync(status);

        return await GetStatusAsync();
    }

    public async Task<PoolStatusDto> SetCapacityAsync(int value)
    {
        var status = await _context.PoolStatus.FirstOrDefaultAsync();
        if (status == null) return await GetStatusAsync();

        value = Math.Max(1, value); // Mínimo 1
        status.MaxCapacity = value;
        status.CurrentCount = Math.Min(status.CurrentCount, value); // Ajustar se necessário
        status.LastUpdated = DateTime.UtcNow;
        await _repository.UpdateAsync(status);

        return await GetStatusAsync();
    }

    public async Task<PoolStatusDto> SetOpenStatusAsync(bool isOpen)
    {
        var status = await _context.PoolStatus.FirstOrDefaultAsync();
        if (status == null) return await GetStatusAsync();

        var wasOpen = status.IsOpen;
        status.IsOpen = isOpen;
        
        // Se está a fechar, resetar contagem
        if (!isOpen)
        {
            status.CurrentCount = 0;
        }
        
        status.LastUpdated = DateTime.UtcNow;
        await _repository.UpdateAsync(status);

        // Se está a fechar a piscina, desativar todos os trabalhadores e gerar relatório
        if (wasOpen && !isOpen)
        {
            try
            {
                if (_workerService != null)
                {
                    await _workerService.DeactivateAllWorkersAsync();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the operation
                Console.WriteLine($"Error deactivating workers: {ex.Message}");
            }
            
            try
            {
                if (_reportService != null)
                {
                    await _reportService.GenerateDailyReportAsync(DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the operation
                Console.WriteLine($"Error generating report: {ex.Message}");
            }
        }

        return await GetStatusAsync();
    }

    public async Task<PoolStatusDto> ResetAsync()
    {
        var status = await _context.PoolStatus.FirstOrDefaultAsync();
        if (status == null) return await GetStatusAsync();

        status.CurrentCount = 0;
        status.IsOpen = false;
        status.LastUpdated = DateTime.UtcNow;
        await _repository.UpdateAsync(status);

        return await GetStatusAsync();
    }
}

