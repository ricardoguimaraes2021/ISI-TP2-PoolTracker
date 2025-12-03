using System.Collections.Generic;
using System.ServiceModel;
using PoolTracker.SOAP.DataContracts;

namespace PoolTracker.SOAP.Contracts;

[ServiceContract]
public interface IWaterQualityDataService
{
    [OperationContract]
    List<WaterQualityData> GetHistory(string poolType);

    [OperationContract]
    WaterQualityData GetLatest(string poolType);

    [OperationContract]
    void RecordMeasurement(WaterQualityData measurement);
}

