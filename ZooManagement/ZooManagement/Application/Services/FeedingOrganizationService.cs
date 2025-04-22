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

    private static async Task<IResult> CompleteFeeding(
    string idString,
    IFeedingScheduleRepository repo)
    {
        if (idString == "0")
        {
            idString = Guid.Empty.ToString();
        }

        if (!Guid.TryParse(idString, out var id))
        {
            return Results.BadRequest("Invalid GUID format");
        }

        var schedule = await repo.GetByIdAsync(id);
        if (schedule is null)
        {
            return Results.NotFound($"Feeding schedule with ID {id} not found");
        }

        schedule.MarkAsCompleted();
        await repo.UpdateAsync(schedule);

        return Results.Ok(new
        {
            Message = "Feeding completed",
            ScheduleId = schedule.Id
        });
    }
}