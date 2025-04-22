using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTOs;

public static class FeedingScheduleEndpoints
{
    private static FoodType GetFoodType(string foodName)
    {
        try
        {
            return FoodType.FromString(foodName);
        }
        catch
        {
            return FoodType.GetRandom();
        }
    }



    private static FeedingTime GetFeedingTime(string time)
    {
        try
        {
            return new FeedingTime(TimeOnly.Parse(time));
        }
        catch
        {
            return new FeedingTime(TimeOnly.FromDateTime(DateTime.Now));
        }
    }


    public static void MapFeedingScheduleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/feeding-schedules")
            .WithTags("Feeding Schedules");


        group.MapGet("/", async (IFeedingScheduleRepository repo) =>
            Results.Ok(await repo.GetAllAsync()))
            .WithName("GetAllFeedingSchedules")
            .Produces<List<FeedingSchedule>>();



        group.MapPost("/", async (
    [FromBody] FeedingScheduleDto dto,
    IFeedingScheduleRepository repo,
    IAnimalRepository animalRepo,
    IDomainEventDispatcher dispatcher) =>
        {
            var animal = await animalRepo.GetByIdAsync(dto.AnimalId);
            if (animal is null) return Results.NotFound("Animal not found");

            TimeOnly time;
            try
            {
                time = TimeOnly.Parse(dto.Time);
            }
            catch
            {
                time = TimeOnly.FromDateTime(DateTime.Now);
            }

            var foodType = GetFoodType(dto.FoodType);

            var schedule = new FeedingSchedule(
                animal,
                new FeedingTime(time),
                foodType,
                dispatcher);

            await repo.AddAsync(schedule);

            return Results.Created($"/feeding-schedules/{schedule.Id}", new
            {
                schedule.Id,
                Time = time.ToString("HH:mm"),
                FoodType = foodType.Name,
                Animal = animal.Name
            });
        })
.WithName("AddFeedingSchedule")
.Accepts<FeedingScheduleDto>("application/json")
.Produces(201)
.Produces(400)
.Produces(404);






        group.MapPost("/{id}/complete", async (
            Guid id,
            IFeedingScheduleRepository repo) =>
        {
            var schedule = await repo.GetByIdAsync(id);
            if (schedule is null) return Results.NotFound();

            schedule.MarkAsCompleted();
            await repo.UpdateAsync(schedule);
            return Results.Ok();
        })
        .WithName("CompleteFeeding")
        .Produces(200)
        .Produces(404);
    }
}
