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

namespace Domain.Test;

 
public class EnclosureTests
{
    [Fact]
    public void Enclosure_Created_WithDefaultValues()
    {
        var enclosure = new Enclosure(EnclosureType.Predator, 100.5, 10);

        Assert.Equal("Predator", enclosure.Type.Name);
        Assert.Equal(100.5, enclosure.Size);
        Assert.Equal(10, enclosure.MaxCapacity);
    }

    [Fact]
    public void AddAnimal_IncreasesAnimalCount()
    {
        var enclosure = new Enclosure(EnclosureType.Herbivore, 50, 5);
        var animal = new Animal(
            AnimalSpecies.Create("Deer"),
            "Bambi",
            DateOnly.Parse("2023-01-01"),
            "Male",
            FoodType.Vegetables,
            new FakeDispatcher()
        );

        enclosure.AddAnimal(animal);

        Assert.Single(enclosure.Animals);
    }
}