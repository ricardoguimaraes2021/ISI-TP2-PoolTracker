using System.Collections.Generic;
using System.ServiceModel;
using PoolTracker.SOAP.DataContracts;

namespace PoolTracker.SOAP.Contracts;

[ServiceContract]
public interface IWorkerDataService
{
    [OperationContract]
    List<WorkerData> GetAllWorkers();

    [OperationContract]
    WorkerData GetWorkerById(int id);

    [OperationContract]
    int CreateWorker(WorkerData worker);

    [OperationContract]
    void UpdateWorker(WorkerData worker);

    [OperationContract]
    void DeleteWorker(int id);
}

