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
    Task<bool> ActivateShiftAsync(string workerId, ShiftType shiftType);
    Task<bool> DeactivateShiftAsync(string workerId);
    Task DeactivateAllWorkersAsync();
    Task<ActiveWorkersResponse> GetActiveWorkersAsync();
    Task<Dictionary<string, int>> GetActiveWorkersCountAsync();
}

