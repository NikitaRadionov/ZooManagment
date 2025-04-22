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
public class InMemoryAnimalRepositoryTests
{
    [Fact]
    public async Task AddAsync_AddsAnimalToRepository()
    {
        var repo = new InMemoryAnimalRepository();
        var animal = new Animal(
            AnimalSpecies.Create("Elephant"),
            "Dumbo",
            DateOnly.Parse("2021-04-10"),
            "Male",
            FoodType.Vegetables,
            new FakeDispatcher()
        );

        await repo.AddAsync(animal);
        var retrieved = await repo.GetByIdAsync(animal.Id);

        Assert.NotNull(retrieved);
        Assert.Equal("Elephant", retrieved.Species.Value);
    }
}