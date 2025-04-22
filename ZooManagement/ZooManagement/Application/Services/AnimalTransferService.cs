using Domain.Entities;
using Domain.Interfaces;
using Domain.Events;

namespace Application.Services;
public sealed class AnimalTransferService
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IEnclosureRepository _enclosureRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public AnimalTransferService(
        IAnimalRepository animalRepository,
        IEnclosureRepository enclosureRepository,
        IDomainEventDispatcher eventDispatcher)
    {
        _animalRepository = animalRepository;
        _enclosureRepository = enclosureRepository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task TransferAnimalAsync(Animal animal, Enclosure newEnclosure)
    {
        if (newEnclosure.Type.Name == "Predator" && animal.Species.Value == "Rabbit")
            throw new InvalidOperationException("Rabbits cannot be placed with predators");

        animal.MoveToEnclosure(newEnclosure);

        await _animalRepository.UpdateAsync(animal);
        await _enclosureRepository.UpdateAsync(newEnclosure);

        await _eventDispatcher.Dispatch(new AnimalMovedEvent(animal, newEnclosure));
    }


}