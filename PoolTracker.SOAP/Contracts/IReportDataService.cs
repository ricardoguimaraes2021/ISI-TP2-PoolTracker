using System;
using System.Collections.Generic;
using System.ServiceModel;
using PoolTracker.SOAP.DataContracts;

namespace PoolTracker.SOAP.Contracts;

[ServiceContract]
public interface IReportDataService
{
    [OperationContract]
    List<DailyReportData> GetReports(DateTime startDate, DateTime endDate);

    [OperationContract]
    DailyReportData GenerateReport(DateTime date);
}

