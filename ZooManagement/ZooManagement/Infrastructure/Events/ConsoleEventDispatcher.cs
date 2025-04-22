using Domain.Interfaces;
using Domain.Events;

namespace Infrastructure.Events
{
    public class ConsoleEventDispatcher : IDomainEventDispatcher
    {
        public Task Dispatch(DomainEvent domainEvent)
        {
            Console.WriteLine($"[Event] {domainEvent.GetType().Name}: {domainEvent.OccurredOn:HH:mm:ss}");

            if (domainEvent is AnimalMovedEvent movedEvent)
            {
                Console.WriteLine($"Animal {movedEvent.Animal.Name} moved to enclosure {movedEvent.NewEnclosure.Type.Name}");
            }

            return Task.CompletedTask;
        }
    }
}