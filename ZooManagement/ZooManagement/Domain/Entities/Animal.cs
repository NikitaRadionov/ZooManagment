using Domain.ValueObjects;
using Domain.Interfaces;
using Domain.Events;


namespace Domain.Entities
{
    public class Animal
    {
        public int Id { get; set; }
        public AnimalSpecies Species { get; private set; }
        public string Name { get; private set; }
        public DateOnly BirthDate { get; private set; }
        public string Gender { get; private set; }
        public FoodType FavoriteFood { get; private set; }
        public bool IsHealthy { get; private set; } = true;
        public Enclosure? CurrentEnclosure { get; private set; }
        private readonly IDomainEventDispatcher _dispatcher;

        private Animal() { }
        public Animal(
            AnimalSpecies species,
            string name,
            DateOnly birthDate,
            string gender,
            FoodType favoriteFood,
            IDomainEventDispatcher dispatcher)
        {
            Species = species;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            BirthDate = birthDate;
            Gender = gender;
            FavoriteFood = favoriteFood;
            _dispatcher = dispatcher;
        }

        public void MoveToEnclosure(Enclosure enclosure)
        {
            if (enclosure.Type.Name == "Predators" && Species.Value == "Rabbit")
                throw new InvalidOperationException("Rabbits should not be placed with predators!");

            CurrentEnclosure?.RemoveAnimal(this);
            enclosure.AddAnimal(this);
            CurrentEnclosure = enclosure;
            _dispatcher.Dispatch(new AnimalMovedEvent(this, enclosure));
        }
        public void Feed(FoodType food)
        {
            if (food != FavoriteFood)
                Console.WriteLine($"{Name} eat {food.Name} reluctantly");
            else
                Console.WriteLine($"{Name} eat {food.Name} with pleasure");
        }

        public void Heal()
        {
            if (!IsHealthy)
            {
                IsHealthy = true;
                Console.WriteLine($"{Name} is cured");
            }
        }

    }
}

