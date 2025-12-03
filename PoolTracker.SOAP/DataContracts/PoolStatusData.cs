using System.Runtime.Serialization;

namespace PoolTracker.SOAP.DataContracts;

[DataContract]
public class PoolStatusData
{
    [DataMember]
    public int CurrentCount { get; set; }

    [DataMember]
    public int MaxCapacity { get; set; }

    [DataMember]
    public bool IsOpen { get; set; }

    [DataMember]
    public DateTime LastUpdated { get; set; }

    [DataMember]
    public string LocationName { get; set; } = string.Empty;

    [DataMember]
    public string Address { get; set; } = string.Empty;

    [DataMember]
    public string Phone { get; set; } = string.Empty;
}

