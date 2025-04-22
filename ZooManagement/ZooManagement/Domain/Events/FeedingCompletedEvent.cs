using Domain.Entities;

namespace Domain.Events
{
    public class FeedingCompletedEvent : DomainEvent
    {
        public FeedingSchedule Schedule { get; }

        public FeedingCompletedEvent(FeedingSchedule schedule)
        {
            Schedule = schedule;
        }


    }
}