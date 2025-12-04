using PoolTracker.Core.Entities;

namespace PoolTracker.Core.DTOs;

/// <summary>
/// Representa um trabalhador da piscina
/// </summary>
public class WorkerDto
{
    /// <summary>ID interno do trabalhador</summary>
    public int Id { get; set; }
    
    /// <summary>Código único do trabalhador (ex: W0001)</summary>
    public string WorkerId { get; set; } = string.Empty;
    
    /// <summary>Nome do trabalhador</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Função do trabalhador (NadadorSalvador, Bar, Vigilante, Bilheteira)</summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>Indica se o trabalhador está ativo no sistema</summary>
    public bool IsActive { get; set; }
    
    /// <summary>Indica se o trabalhador está atualmente em turno</summary>
    public bool OnShift { get; set; }
    
    /// <summary>Tipo de turno atual (Manha ou Tarde), null se não está em turno</summary>
    public string? CurrentShiftType { get; set; }
    
    /// <summary>Data/hora de início do turno atual, null se não está em turno</summary>
    public DateTime? ShiftStartTime { get; set; }
    
    /// <summary>Data de criação do registo</summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>Data da última atualização</summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request para criar um novo trabalhador
/// </summary>
public class CreateWorkerRequest
{
    /// <summary>Código do trabalhador (opcional, gerado automaticamente se não fornecido)</summary>
    public string? WorkerId { get; set; }
    
    /// <summary>Nome do trabalhador (obrigatório)</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Função do trabalhador: NadadorSalvador, Bar, Vigilante ou Bilheteira</summary>
    public WorkerRole Role { get; set; }
}

/// <summary>
/// Request para atualizar um trabalhador existente
/// </summary>
public class UpdateWorkerRequest
{
    /// <summary>Novo nome do trabalhador (opcional)</summary>
    public string? Name { get; set; }
    
    /// <summary>Nova função do trabalhador (opcional)</summary>
    public WorkerRole? Role { get; set; }
    
    /// <summary>Ativar/desativar trabalhador (opcional)</summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// Request para ativar turno de um trabalhador
/// </summary>
public class ActivateShiftRequest
{
    /// <summary>Tipo de turno (opcional). Se não especificado, é determinado automaticamente baseado na hora atual</summary>
    public ShiftType? ShiftType { get; set; }
}

/// <summary>
/// Resposta com lista de trabalhadores ativos
/// </summary>
public class ActiveWorkersResponse
{
    /// <summary>Contagem de trabalhadores ativos por função</summary>
    public Dictionary<string, int> Counts { get; set; } = new();
    
    /// <summary>Lista de trabalhadores atualmente em turno</summary>
    public List<WorkerDto> Workers { get; set; } = new();
}

/// <summary>
/// Estatísticas de turnos de um trabalhador
/// </summary>
public class ShiftStatsDto
{
    /// <summary>Código único do trabalhador</summary>
    public string WorkerId { get; set; } = string.Empty;
    
    /// <summary>Nome do trabalhador</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Função do trabalhador</summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>Número de turnos da manhã no período</summary>
    public int Manha { get; set; }
    
    /// <summary>Número de turnos da tarde no período</summary>
    public int Tarde { get; set; }
    
    /// <summary>Total de turnos no período</summary>
    public int Total { get; set; }
}

/// <summary>
/// Representa um turno de um trabalhador
/// </summary>
public class WorkerShiftDto
{
    /// <summary>ID do registo de turno</summary>
    public int Id { get; set; }
    
    /// <summary>Código do trabalhador</summary>
    public string WorkerId { get; set; } = string.Empty;
    
    /// <summary>Nome do trabalhador</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Função do trabalhador</summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>Tipo de turno (Manha ou Tarde)</summary>
    public string? ShiftType { get; set; }
    
    /// <summary>Data/hora de início do turno</summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>Data/hora de fim do turno (null se ainda em curso)</summary>
    public DateTime? EndTime { get; set; }
}
