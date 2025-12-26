using System.Runtime.Serialization;

namespace PoolTracker.SOAP.DataContracts;

[DataContract]
public class WaterQualityData
{
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string PoolType { get; set; } = string.Empty;

    [DataMember]
    public decimal PhLevel { get; set; }

    [DataMember]
    public decimal Temperature { get; set; }

    [DataMember]
    public DateTime MeasuredAt { get; set; }

    [DataMember]
    public string? Notes { get; set; }
}

