using Domain.Events;
using Domain.ValueObjects;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class FeedingSchedule
    {
        public Guid Id { get; private set; }
        public Animal Animal { get; private set; }
        public FeedingTime Time { get; private set; }
        public FoodType Food { get; private set; }
        public bool IsCompleted { get; private set; }

        private readonly IDomainEventDispatcher _dispatcher;

        public FeedingSchedule(Animal animal, FeedingTime time, FoodType food, IDomainEventDispatcher dispatcher)
        {
            Animal = animal ?? throw new ArgumentNullException(nameof(animal));
            Time = time;
            Food = food;
            _dispatcher = dispatcher;
        }

        public void CheckAndDispatchFeedingTime(TimeOnly currentTime)
        {
            if (IsCompleted && Time.Time == currentTime)
            {
                _dispatcher.Dispatch(new FeedingTimeEvent(this));
            }
        }

        public void Reschedule(FeedingTime newTime) => Time = newTime;

        public void MarkAsCompleted()
        {
            if (IsCompleted) return;

            IsCompleted = true;
            _dispatcher.Dispatch(new FeedingCompletedEvent(this));
        }
    }
}