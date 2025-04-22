
using Domain.Entities;
using Domain.Interfaces;
namespace Application.Services;
public sealed class ZooStatisticsService
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IEnclosureRepository _enclosureRepository;

    public ZooStatisticsService(
        IAnimalRepository animalRepository,
        IEnclosureRepository enclosureRepository)
    {
        _animalRepository = animalRepository;
        _enclosureRepository = enclosureRepository;
    }

    public async Task<ZooStatistics> GetStatisticsAsync()
    {
        var animals = await _animalRepository.GetAllAsync();
        var enclosures = await _enclosureRepository.GetAllAsync();

        return new ZooStatistics(
            TotalAnimals: animals.Count,
            TotalEnclosures: enclosures.Count,
            FreeEnclosures: enclosures.Count(e => e.Animals.Count == 0),
            AverageAnimalsPerEnclosure: enclosures.Any()
        ? enclosures.Average(e => e.Animals.Count)
                : 0);
    }
}

public record ZooStatistics(
    int TotalAnimals,
    int TotalEnclosures,
    int FreeEnclosures,
    double AverageAnimalsPerEnclosure);