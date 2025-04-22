namespace Domain.Events;
using Domain.Entities;
public class FeedingTimeEvent : DomainEvent
{
    public FeedingSchedule Schedule { get; }

    public FeedingTimeEvent(FeedingSchedule schedule)
    {
        Schedule = schedule;
    }
}