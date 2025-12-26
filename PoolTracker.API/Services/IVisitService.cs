namespace PoolTracker.API.Services;

public interface IVisitService
{
    Task IncrementDailyVisitorsAsync();
    Task<int> GetTodayVisitorsAsync();
}

