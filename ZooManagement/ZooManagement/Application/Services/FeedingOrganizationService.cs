using Domain.Entities;
using Domain.Interfaces;
using Domain.Events;
using Domain.ValueObjects;
namespace Application.Services;
public sealed class FeedingOrganizationService
{
    private readonly IFeedingScheduleRepository _feedingScheduleRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public FeedingOrganizationService(
        IFeedingScheduleRepository feedingScheduleRepository,
        IDomainEventDispatcher eventDispatcher)
    {
        _feedingScheduleRepository = feedingScheduleRepository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task ScheduleFeedingAsync(
        Animal animal,
        TimeOnly time,
        FoodType foodType)
    {
        var schedule = new FeedingSchedule(
            animal,
            new FeedingTime(time),
            foodType,
            _eventDispatcher);

        await _feedingScheduleRepository.AddAsync(schedule);
    }

    public async Task CompleteFeedingAsync(Guid scheduleId)
    {
        var schedule = await _feedingScheduleRepository.GetByIdAsync(scheduleId);
        schedule.MarkAsCompleted();
        await _feedingScheduleRepository.UpdateAsync(schedule);
    }
}