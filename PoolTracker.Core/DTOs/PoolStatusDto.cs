namespace PoolTracker.Core.DTOs;

/// <summary>
/// Estado atual da piscina
/// </summary>
public class PoolStatusDto
{
    /// <summary>Número atual de pessoas na piscina</summary>
    public int CurrentCount { get; set; }
    
    /// <summary>Capacidade máxima permitida</summary>
    public int MaxCapacity { get; set; }
    
    /// <summary>Indica se a piscina está aberta</summary>
    public bool IsOpen { get; set; }
    
    /// <summary>Data/hora da última atualização</summary>
    public DateTime LastUpdated { get; set; }
    
    /// <summary>Nome da localização</summary>
    public string LocationName { get; set; } = string.Empty;
    
    /// <summary>Morada da piscina</summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>Contacto telefónico</summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>Horário de funcionamento para hoje</summary>
    public string TodayOpeningHours { get; set; } = string.Empty;
}

/// <summary>
/// Request para definir contagem manual
/// </summary>
public class SetCountRequest
{
    /// <summary>Número de pessoas (será ajustado se ultrapassar os limites)</summary>
    public int Value { get; set; }
}

/// <summary>
/// Request para definir capacidade máxima
/// </summary>
public class SetCapacityRequest
{
    /// <summary>Nova capacidade máxima (mínimo: 1)</summary>
    public int Value { get; set; }
}

/// <summary>
/// Request para abrir/fechar a piscina
/// </summary>
public class SetOpenStatusRequest
{
    /// <summary>true para abrir, false para fechar</summary>
    public bool IsOpen { get; set; }
}

