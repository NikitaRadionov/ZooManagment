using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Xunit; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Domain.Entities;
using Domain.ValueObjects;
using Domain.Interfaces;
using Domain.Events;
using Application.Services;
using Infrastructure.Repositories;
using Presentation.DTOs;

public class AnimalTransferServiceTests
{
    [Fact]
    public async Task TransferAnimalAsync_UpdatesEnclosure()
    {
        var animalRepo = new InMemoryAnimalRepository();
        var enclosureRepo = new InMemoryEnclosureRepository();
        var dispatcher = new FakeDispatcher();
        
        var service = new AnimalTransferService(
            animalRepo,
            enclosureRepo,
            dispatcher
        );

        var animal = new Animal(
            AnimalSpecies.Create("Zebra"),
            "Stripes",
            DateOnly.Parse("2022-06-20"),
            "Female",
            FoodType.Grains,
            dispatcher
        );
        var oldEnclosure = new Enclosure(EnclosureType.Herbivore, 200, 10);
        var newEnclosure = new Enclosure(EnclosureType.Herbivore, 300, 15);

        await animalRepo.AddAsync(animal);
        await enclosureRepo.AddAsync(newEnclosure);
        
        await service.TransferAnimalAsync(animal, newEnclosure);

        Assert.Equal(newEnclosure.Id, animal.CurrentEnclosure?.Id);
    }
}