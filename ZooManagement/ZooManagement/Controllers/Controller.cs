
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Domain.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("/animals")
    .MapAnimalApi()
    .WithTags("Animals");

app.MapGroup("/enclosures")
    .MapEnclosureApi()
    .WithTags("Enclosures");

app.MapGroup("/feeding-schedules")
    .MapFeedingScheduleApi()
    .WithTags("Feeding Schedules");

var animalsGroup = app.MapGroup("/animals")
    .WithTags("Animals");

animalsGroup.MapDelete("/{id}", async (int id, IAnimalRepository repo) =>
{
    await repo.DeleteAsync(id);
    return Results.NoContent();
})
.WithName("DeleteAnimal")
.Produces(204);

var enclosuresGroup = app.MapGroup("/enclosures")
    .WithTags("Enclosures");

enclosuresGroup.MapDelete("/{id}", async (int id, IEnclosureRepository repo) =>
{
    await repo.DeleteAsync(id);
    return Results.NoContent();
})
.WithName("DeleteEnclosure")
.Produces(204);

var feedingSchedulesGroup = app.MapGroup("/feeding-schedules")
    .WithTags("Feeding Schedules");

feedingSchedulesGroup.MapGet("/", async (IFeedingScheduleRepository repo) =>
    Results.Ok(await repo.GetAllAsync()))
    .WithName("GetAllFeedingSchedules")
    .Produces<List<FeedingSchedule>>();


app.Run();


public static class ApiEndpoints
{
    public static RouteGroupBuilder MapAnimalApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (ZooStatisticsService statsService) =>
            Results.Ok(await statsService.GetStatisticsAsync()))
            .WithName("GetZooStatistics");

        group.MapGet("/all", async (IAnimalRepository repo) =>
            Results.Ok(await repo.GetAllAsync()))
            .WithName("GetAllAnimals");


        group.MapPost("/", async (
    [FromBody] AnimalDto dto,
    IAnimalRepository repo,
    IDomainEventDispatcher dispatcher) =>
        {
            try
            {
                DateOnly birthDate;
                try
                {
                    birthDate = DateOnly.Parse(dto.BirthDate);
                }
                catch
                {
                    birthDate = DateOnly.FromDateTime(DateTime.Today);
                }

                var foodType = GetFoodType(dto.FavoriteFood);

                var animal = new Animal(
                    AnimalSpecies.Create(dto.Species),
                    dto.Name,
                    birthDate,
                    dto.Gender,
                    foodType,
                    dispatcher);

                animal.Heal();
                await repo.AddAsync(animal);

                return Results.Created($"/animals/{animal.Id}", new
                {
                    animal.Id,
                    dto.Species,
                    AssignedBirthDate = birthDate.ToString("yyyy-MM-dd"),
                    AssignedFoodType = foodType.Name
                });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .WithName("AddAnimal")
        .Accepts<AnimalDto>("application/json")
        .Produces(201)
        .Produces(400);





        group.MapPost("/transfer", async (
        [FromBody] TransferRequest request,
        AnimalTransferService service,
        IAnimalRepository animalRepo,
        IEnclosureRepository enclosureRepo) =>
        {
            var animal = await animalRepo.GetByIdAsync(request.AnimalId);
            var enclosure = await enclosureRepo.GetByIdAsync(request.NewEnclosureId);

            if (animal == null || enclosure == null)
            {
                var errors = new List<string>();
                if (animal == null) errors.Add($"Animal with ID {request.AnimalId} not found");
                if (enclosure == null) errors.Add($"Enclosure with ID {request.NewEnclosureId} not found");
                return Results.BadRequest(new { Errors = errors });
            }

            try
            {
                await service.TransferAnimalAsync(animal, enclosure);
                return Results.Ok(new { Message = "Transfer completed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        })
        .WithName("TransferAnimal")
        .Produces(200)
        .Produces(400);

        return group;
    }

    public static RouteGroupBuilder MapEnclosureApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IEnclosureRepository repo) =>
            Results.Ok(await repo.GetAllAsync()))
            .WithName("GetAllEnclosures");

        group.MapPost("/", async (
            [FromBody] EnclosureDto dto,
            IEnclosureRepository repo) =>
        {
            var enclosureType = GetEnclosureType(dto.Type);
            var enclosure = new Enclosure(
                enclosureType,
                dto.Size,
                dto.MaxCapacity);

            await repo.AddAsync(enclosure);
            return Results.Created($"/enclosures/{enclosure.Id}", enclosure);
        })
        .WithName("AddEnclosure")
        .Accepts<EnclosureDto>("application/json")
        .Produces<Enclosure>(201);

        return group;
    }

    public static RouteGroupBuilder MapFeedingScheduleApi(this RouteGroupBuilder group)
    {
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
                AnimalName = animal.Name
            });
        })
        .WithName("AddFeedingSchedule")
        .Accepts<FeedingScheduleDto>("application/json")
        .Produces(201)
        .Produces(400)
        .Produces(404);

        return group;
    }

    private static EnclosureType GetEnclosureType(string typeName)
    {
        try
        {
            return EnclosureType.FromString(typeName);
        }
        catch
        {
            return EnclosureType.GetRandomType();
        }
    }



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

}

public record AnimalDto(
    [Required] string Species,
    [Required] string Name,
    string BirthDate,
    [Required] string Gender,
    string FavoriteFood,
    bool IsHealthy)
{
    public string BirthDate { get; init; } = BirthDate;
}


public record EnclosureDto(
    string Type,
    double Size,
    int MaxCapacity);

public record FeedingScheduleDto(
    int AnimalId,
    string Time,
    string FoodType);



public record TransferRequest(
    [Range(0, int.MaxValue, ErrorMessage = "Animal ID must be non-negative")]
    int AnimalId,

    [Range(0, int.MaxValue, ErrorMessage = "Enclosure ID must be non-negative")]
    int NewEnclosureId
);

