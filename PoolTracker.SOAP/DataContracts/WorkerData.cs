using System.Runtime.Serialization;

namespace PoolTracker.SOAP.DataContracts;

[DataContract]
public class WorkerData
{
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string WorkerId { get; set; } = string.Empty;

    [DataMember]
    public string Name { get; set; } = string.Empty;

    [DataMember]
    public string Role { get; set; } = string.Empty;

    [DataMember]
    public bool IsActive { get; set; }

    [DataMember]
    public DateTime CreatedAt { get; set; }

    [DataMember]
    public DateTime UpdatedAt { get; set; }
}

