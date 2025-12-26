using Microsoft.EntityFrameworkCore;
using PoolTracker.Core.DTOs;
using PoolTracker.Core.Entities;
using PoolTracker.Core.Interfaces;
using PoolTracker.Infrastructure.Data;

namespace PoolTracker.API.Services;

public class WorkerService : IWorkerService
{
    private readonly IRepository<Worker> _repository;
    private readonly IRepository<ActiveWorker> _activeWorkerRepository;
    private readonly PoolTrackerDbContext _context;

    // Horários dos turnos (em hora local de Portugal)
    private static readonly TimeSpan MorningShiftStart = new TimeSpan(9, 0, 0);
    private static readonly TimeSpan MorningShiftEnd = new TimeSpan(14, 0, 0);
    private static readonly TimeSpan AfternoonShiftStart = new TimeSpan(14, 0, 0);
    private static readonly TimeSpan AfternoonShiftEnd = new TimeSpan(19, 0, 0);

    public WorkerService(
        IRepository<Worker> repository,
        IRepository<ActiveWorker> activeWorkerRepository,
        PoolTrackerDbContext context)
    {
        _repository = repository;
        _activeWorkerRepository = activeWorkerRepository;
        _context = context;
    }

    private async Task<string> GenerateWorkerIdAsync()
    {
        var lastWorker = await _context.Workers
            .Where(w => w.WorkerId.StartsWith("W"))
            .OrderByDescending(w => w.WorkerId)
            .FirstOrDefaultAsync();

        int nextId = 1;
        if (lastWorker != null)
        {
            if (lastWorker.WorkerId.Length > 1 && int.TryParse(lastWorker.WorkerId.Substring(1), out int lastId))
            {
                nextId = lastId + 1;
            }
        }

        return $"W{nextId:D4}";
    }

    /// <summary>
    /// Obtém a hora local de Portugal (Europe/Lisbon)
    /// </summary>
    private DateTime GetPortugalTime()
    {
        var utcNow = DateTime.UtcNow;
        try
        {
            var portugalTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Lisbon");
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, portugalTimeZone);
        }
        catch
        {
            // Fallback para Windows (nome diferente do timezone)
            try
            {
                var portugalTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(utcNow, portugalTimeZone);
            }
            catch
            {
                // Se falhar, assume UTC (Portugal está em UTC no inverno)
                return utcNow;
            }
        }
    }

    /// <summary>
    /// Determina automaticamente o tipo de turno baseado na hora local de Portugal
    /// Manhã: 9:00 - 14:00
    /// Tarde: 14:00 - 19:00
    /// </summary>
    public ShiftType DetermineCurrentShiftType()
    {
        var portugalTime = GetPortugalTime();
        var currentTime = portugalTime.TimeOfDay;
        
        // Se estiver antes das 14:00, é turno da manhã
        // Se estiver a partir das 14:00, é turno da tarde
        if (currentTime >= MorningShiftStart && currentTime < MorningShiftEnd)
        {
            return ShiftType.Manha;
        }
        else
        {
            return ShiftType.Tarde;
        }
    }

    /// <summary>
    /// Verifica se está dentro do horário válido para iniciar turno (9:00 - 19:00)
    /// </summary>
    public bool IsValidShiftTime()
    {
        var portugalTime = GetPortugalTime();
        var currentTime = portugalTime.TimeOfDay;
        
        return currentTime >= MorningShiftStart && currentTime < AfternoonShiftEnd;
    }

    public async Task<List<WorkerDto>> GetAllWorkersAsync(bool activeOnly = false)
    {
        var query = _context.Workers
            .GroupJoin(
                _context.ActiveWorkers.Where(aw => aw.EndTime == null),
                w => w.WorkerId,
                aw => aw.WorkerId,
                (w, aws) => new { Worker = w, ActiveWorkers = aws })
            .SelectMany(
                x => x.ActiveWorkers.DefaultIfEmpty(),
                (x, aw) => new { x.Worker, ActiveWorker = aw, OnShift = aw != null });

        if (activeOnly)
        {
            query = query.Where(x => x.Worker.IsActive);
        }

        var results = await query
            .OrderBy(x => x.Worker.Name)
            .ToListAsync();

        return results.Select(x => new WorkerDto
        {
            Id = x.Worker.Id,
            WorkerId = x.Worker.WorkerId,
            Name = x.Worker.Name,
            Role = x.Worker.Role.ToString(),
            IsActive = x.Worker.IsActive,
            OnShift = x.OnShift,
            CurrentShiftType = x.ActiveWorker?.ShiftType?.ToString(),
            ShiftStartTime = x.ActiveWorker?.StartTime,
            CreatedAt = x.Worker.CreatedAt,
            UpdatedAt = x.Worker.UpdatedAt
        }).ToList();
    }

    public async Task<WorkerDto?> GetWorkerByIdAsync(int id)
    {
        var worker = await _repository.GetByIdAsync(id);
        if (worker == null) return null;

        var activeWorker = await _context.ActiveWorkers
            .FirstOrDefaultAsync(aw => aw.WorkerId == worker.WorkerId && aw.EndTime == null);

        return new WorkerDto
        {
            Id = worker.Id,
            WorkerId = worker.WorkerId,
            Name = worker.Name,
            Role = worker.Role.ToString(),
            IsActive = worker.IsActive,
            OnShift = activeWorker != null,
            CurrentShiftType = activeWorker?.ShiftType?.ToString(),
            ShiftStartTime = activeWorker?.StartTime,
            CreatedAt = worker.CreatedAt,
            UpdatedAt = worker.UpdatedAt
        };
    }

    public async Task<WorkerDto?> GetWorkerByWorkerIdAsync(string workerId)
    {
        var worker = await _context.Workers
            .FirstOrDefaultAsync(w => w.WorkerId == workerId);
        
        if (worker == null) return null;

        var activeWorker = await _context.ActiveWorkers
            .FirstOrDefaultAsync(aw => aw.WorkerId == workerId && aw.EndTime == null);

        return new WorkerDto
        {
            Id = worker.Id,
            WorkerId = worker.WorkerId,
            Name = worker.Name,
            Role = worker.Role.ToString(),
            IsActive = worker.IsActive,
            OnShift = activeWorker != null,
            CurrentShiftType = activeWorker?.ShiftType?.ToString(),
            ShiftStartTime = activeWorker?.StartTime,
            CreatedAt = worker.CreatedAt,
            UpdatedAt = worker.UpdatedAt
        };
    }

    public async Task<WorkerDto> CreateWorkerAsync(CreateWorkerRequest request)
    {
        var workerId = request.WorkerId ?? await GenerateWorkerIdAsync();

        var worker = new Worker
        {
            WorkerId = workerId,
            Name = request.Name,
            Role = request.Role,
            IsActive = true
        };

        await _repository.AddAsync(worker);

        return new WorkerDto
        {
            Id = worker.Id,
            WorkerId = worker.WorkerId,
            Name = worker.Name,
            Role = worker.Role.ToString(),
            IsActive = worker.IsActive,
            OnShift = false,
            CreatedAt = worker.CreatedAt,
            UpdatedAt = worker.UpdatedAt
        };
    }

    public async Task<WorkerDto?> UpdateWorkerAsync(string workerId, UpdateWorkerRequest request)
    {
        var worker = await _context.Workers
            .FirstOrDefaultAsync(w => w.WorkerId == workerId);
        
        if (worker == null) return null;

        if (request.Name != null) worker.Name = request.Name;
        if (request.Role.HasValue) worker.Role = request.Role.Value;
        if (request.IsActive.HasValue) worker.IsActive = request.IsActive.Value;
        
        worker.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(worker);

        return await GetWorkerByWorkerIdAsync(workerId);
    }

    public async Task<bool> DeleteWorkerAsync(string workerId)
    {
        var worker = await _context.Workers
            .FirstOrDefaultAsync(w => w.WorkerId == workerId);
        
        if (worker == null) return false;

        worker.IsActive = false;
        worker.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(worker);

        return true;
    }

    public async Task<ActivateShiftResult> ActivateShiftAsync(string workerId, ShiftType? shiftType = null)
    {
        // 1. Verificar se a piscina está aberta
        var poolStatus = await _context.PoolStatus.FirstOrDefaultAsync();
        if (poolStatus == null || !poolStatus.IsOpen)
        {
            return new ActivateShiftResult
            {
                Success = false,
                ErrorMessage = "Não é possível iniciar turno com a piscina fechada."
            };
        }

        // 2. Verificar se está dentro do horário válido (9:00 - 19:00)
        if (!IsValidShiftTime())
        {
            var portugalTime = GetPortugalTime();
            return new ActivateShiftResult
            {
                Success = false,
                ErrorMessage = $"Fora do horário de funcionamento. Os turnos só podem ser iniciados entre 09:00 e 19:00. Hora atual: {portugalTime:HH:mm}"
            };
        }

        // 3. Verificar se já está ativo
        var existing = await _context.ActiveWorkers
            .FirstOrDefaultAsync(aw => aw.WorkerId == workerId && aw.EndTime == null);
        
        if (existing != null)
        {
            return new ActivateShiftResult
            {
                Success = true, // Já está ativo, não é erro
                ShiftType = existing.ShiftType,
                ErrorMessage = null
            };
        }

        // 4. Verificar se o trabalhador existe e está ativo
        var worker = await _context.Workers
            .FirstOrDefaultAsync(w => w.WorkerId == workerId);
        
        if (worker == null)
        {
            return new ActivateShiftResult
            {
                Success = false,
                ErrorMessage = "Trabalhador não encontrado."
            };
        }

        if (!worker.IsActive)
        {
            return new ActivateShiftResult
            {
                Success = false,
                ErrorMessage = "Trabalhador está inativo."
            };
        }

        // 5. Determinar automaticamente o tipo de turno se não especificado
        var determinedShiftType = shiftType ?? DetermineCurrentShiftType();

        // 6. Criar registo de turno ativo
        var activeWorker = new ActiveWorker
        {
            WorkerId = workerId,
            Role = worker.Role,
            ShiftType = determinedShiftType,
            StartTime = DateTime.UtcNow
        };

        await _activeWorkerRepository.AddAsync(activeWorker);
        
        return new ActivateShiftResult
        {
            Success = true,
            ShiftType = determinedShiftType,
            ErrorMessage = null
        };
    }

    public async Task<bool> DeactivateShiftAsync(string workerId)
    {
        var activeWorker = await _context.ActiveWorkers
            .FirstOrDefaultAsync(aw => aw.WorkerId == workerId && aw.EndTime == null);
        
        if (activeWorker == null) return false;

        activeWorker.EndTime = DateTime.UtcNow;
        await _activeWorkerRepository.UpdateAsync(activeWorker);

        return true;
    }

    public async Task DeactivateAllWorkersAsync()
    {
        var activeWorkers = await _context.ActiveWorkers
            .Where(aw => aw.EndTime == null)
            .ToListAsync();

        var now = DateTime.UtcNow;
        foreach (var activeWorker in activeWorkers)
        {
            activeWorker.EndTime = now;
            await _activeWorkerRepository.UpdateAsync(activeWorker);
        }
    }

    public async Task<ActiveWorkersResponse> GetActiveWorkersAsync()
    {
        var activeWorkers = await _context.ActiveWorkers
            .Include(aw => aw.Worker)
            .Where(aw => aw.EndTime == null)
            .OrderBy(aw => aw.Role)
            .ThenBy(aw => aw.Worker!.Name)
            .ToListAsync();

        var counts = activeWorkers
            .GroupBy(aw => aw.Role.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var workers = activeWorkers.Select(aw => new WorkerDto
        {
            Id = aw.Worker!.Id,
            WorkerId = aw.WorkerId,
            Name = aw.Worker.Name,
            Role = aw.Role.ToString(),
            IsActive = aw.Worker.IsActive,
            OnShift = true,
            CurrentShiftType = aw.ShiftType?.ToString(),
            ShiftStartTime = aw.StartTime,
            CreatedAt = aw.Worker.CreatedAt,
            UpdatedAt = aw.Worker.UpdatedAt
        }).ToList();

        return new ActiveWorkersResponse
        {
            Counts = counts,
            Workers = workers
        };
    }

    public async Task<Dictionary<string, int>> GetActiveWorkersCountAsync()
    {
        // Primeiro buscar os dados, depois fazer GroupBy em memória (ToString() não pode ser traduzido para SQL)
        var activeWorkers = await _context.ActiveWorkers
            .Where(aw => aw.EndTime == null)
            .ToListAsync();

        var counts = activeWorkers
            .GroupBy(aw => aw.Role.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        return counts;
    }

    public async Task<List<ShiftStatsDto>> GetShiftStatsAsync(DateTime startDate, DateTime endDate)
    {
        // Buscar todos os turnos completados no período (com EndTime definido)
        var shifts = await _context.ActiveWorkers
            .Include(aw => aw.Worker)
            .Where(aw => aw.StartTime.Date >= startDate.Date && 
                        aw.StartTime.Date <= endDate.Date &&
                        aw.EndTime != null) // Só contar turnos completados
            .ToListAsync();

        // Agrupar por trabalhador e calcular estatísticas
        var result = shifts
            .GroupBy(s => s.WorkerId)
            .Select(g => 
            {
                var firstShift = g.First();
                var morningShifts = g.Count(s => s.ShiftType == ShiftType.Manha);
                var afternoonShifts = g.Count(s => s.ShiftType == ShiftType.Tarde);
                
                return new ShiftStatsDto
                {
                    WorkerId = g.Key,
                    Name = firstShift.Worker?.Name ?? "Desconhecido",
                    Role = firstShift.Worker?.Role.ToString() ?? "Desconhecido",
                    Manha = morningShifts,
                    Tarde = afternoonShifts,
                    Total = morningShifts + afternoonShifts
                };
            })
            .OrderBy(s => s.Name)
            .ToList();

        return result;
    }

    public async Task<List<WorkerShiftDto>> GetWorkerShiftsAsync(string workerId, DateTime startDate, DateTime endDate)
    {
        var shifts = await _context.ActiveWorkers
            .Include(aw => aw.Worker)
            .Where(aw => aw.WorkerId == workerId &&
                        aw.StartTime.Date >= startDate.Date &&
                        aw.StartTime.Date <= endDate.Date &&
                        aw.EndTime != null)
            .OrderByDescending(aw => aw.StartTime)
            .ToListAsync();

        return shifts.Select(aw => new WorkerShiftDto
        {
            Id = aw.Id,
            WorkerId = aw.WorkerId,
            Name = aw.Worker?.Name ?? "",
            Role = aw.Role.ToString(),
            ShiftType = aw.ShiftType?.ToString(),
            StartTime = aw.StartTime,
            EndTime = aw.EndTime
        }).ToList();
    }
}
