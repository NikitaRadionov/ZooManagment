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

public class AnimalTests
{
    [Fact]
    public void CreateAnimal_ValidData_Success()
    {
        var animal = new Animal(
            AnimalSpecies.Create("Lion"),
            "Simba",
            DateOnly.Parse("2020-01-01"),
            "Male",
            FoodType.Meat,
            new FakeDispatcher());
        
        Assert.Equal("Lion", animal.Species.Value);
    }
}