using System.Runtime.Serialization;

namespace PoolTracker.SOAP.DataContracts;

[DataContract]
public class DailyReportData
{
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public DateTime ReportDate { get; set; }

    [DataMember]
    public int TotalVisitors { get; set; }

    [DataMember]
    public int MaxOccupancy { get; set; }

    [DataMember]
    public decimal? AvgOccupancy { get; set; }

    [DataMember]
    public DateTime? OpeningTime { get; set; }

    [DataMember]
    public DateTime ClosingTime { get; set; }

    [DataMember]
    public string? WaterQualityChildren { get; set; }

    [DataMember]
    public string? WaterQualityAdults { get; set; }

    [DataMember]
    public string? ActiveWorkersCount { get; set; }

    [DataMember]
    public string? CleaningRecords { get; set; }

    [DataMember]
    public DateTime CreatedAt { get; set; }
}

