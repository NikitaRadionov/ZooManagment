using Domain.Interfaces;

namespace Infrastructure.Services;

public class FeedingSchedulerService : BackgroundService
{
    private readonly IFeedingScheduleRepository _repository;
    private readonly IDomainEventDispatcher _dispatcher;
    public FeedingSchedulerService(
    IFeedingScheduleRepository repository,
    IDomainEventDispatcher dispatcher)
    {
        _repository = repository;
        _dispatcher = dispatcher;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = TimeOnly.FromDateTime(DateTime.Now);
            var schedules = await _repository.GetAllAsync();

            foreach (var schedule in schedules)
            {
                schedule.CheckAndDispatchFeedingTime(now);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}