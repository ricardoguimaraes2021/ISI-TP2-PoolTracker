using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;

namespace PoolTracker.API.Services;

public interface IWorkerService
{
    Task<List<WorkerDto>> GetAllWorkersAsync(bool activeOnly = false);
    Task<WorkerDto?> GetWorkerByIdAsync(int id);
    Task<WorkerDto?> GetWorkerByWorkerIdAsync(string workerId);
    Task<WorkerDto> CreateWorkerAsync(CreateWorkerRequest request);
    Task<WorkerDto?> UpdateWorkerAsync(string workerId, UpdateWorkerRequest request);
    Task<bool> DeleteWorkerAsync(string workerId);
    Task<ActivateShiftResult> ActivateShiftAsync(string workerId, ShiftType? shiftType = null);
    Task<bool> DeactivateShiftAsync(string workerId);
    Task DeactivateAllWorkersAsync();
    Task<ActiveWorkersResponse> GetActiveWorkersAsync();
    Task<Dictionary<string, int>> GetActiveWorkersCountAsync();
    Task<List<ShiftStatsDto>> GetShiftStatsAsync(DateTime startDate, DateTime endDate);
    Task<List<WorkerShiftDto>> GetWorkerShiftsAsync(string workerId, DateTime startDate, DateTime endDate);
    ShiftType DetermineCurrentShiftType();
    bool IsValidShiftTime();
}

public class ActivateShiftResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public ShiftType? ShiftType { get; set; }
}
