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

 public class FeedingScheduleTests
{
    [Fact]
    public void MarkAsCompleted_SetsIsCompletedTrue()
    {
        var animal = new Animal(
            AnimalSpecies.Create("Penguin"),
            "Skipper",
            DateOnly.Parse("2022-03-15"),
            "Male",
            FoodType.Fish,
            new FakeDispatcher()
        );
        
        var schedule = new FeedingSchedule(
            animal,
            new FeedingTime(TimeOnly.Parse("10:00")),
            FoodType.Fish,
            new FakeDispatcher()
        );

        schedule.MarkAsCompleted();

        Assert.True(schedule.IsCompleted);
    }
}
