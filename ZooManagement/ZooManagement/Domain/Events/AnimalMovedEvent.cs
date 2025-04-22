using Domain.Entities;


namespace Domain.Events
{
    public class AnimalMovedEvent : DomainEvent
    {
        public Animal Animal { get; }
        public Enclosure NewEnclosure { get; }

        public AnimalMovedEvent(Animal animal, Enclosure newEnclosure)
        {
            Animal = animal;
            NewEnclosure = newEnclosure;
        }
    }
}
