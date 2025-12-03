using System.ServiceModel;
using PoolTracker.SOAP.DataContracts;

namespace PoolTracker.SOAP.Contracts;

[ServiceContract]
public interface IPoolDataService
{
    [OperationContract]
    PoolStatusData GetPoolStatus();

    [OperationContract]
    void UpdatePoolStatus(PoolStatusData status);

    [OperationContract]
    void IncrementCount();

    [OperationContract]
    void DecrementCount();
}

