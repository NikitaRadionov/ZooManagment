using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTOs;

namespace Presentation.Endpoints;

public static class AnimalEndpoints
{
    public static RouteGroupBuilder MapAnimalApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (ZooStatisticsService statsService) =>
            Results.Ok(await statsService.GetStatisticsAsync()))
            .WithName("GetZooStatistics");

        group.MapGet("/all", async (IAnimalRepository repo) =>
            Results.Ok(await repo.GetAllAsync()))
            .WithName("GetAllAnimals");

        group.MapPost("/", AddAnimal)
            .WithName("AddAnimal")
            .Accepts<AnimalDto>("application/json")
            .Produces(201)
            .Produces(400);

        group.MapPost("/transfer", TransferAnimal)
            .WithName("TransferAnimal")
            .Produces(200)
            .Produces(400);

        group.MapDelete("/{id}", DeleteAnimal)
            .WithName("DeleteAnimal")
            .Produces(204)
            .Produces(404);

        return group;
    }

    private static async Task<IResult> AddAnimal(
        [FromBody] AnimalDto dto,
        IAnimalRepository repo,
        IDomainEventDispatcher dispatcher)
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
    }

    private static async Task<IResult> TransferAnimal(
        [FromBody] TransferRequestDto request,
        AnimalTransferService service,
        IAnimalRepository animalRepo,
        IEnclosureRepository enclosureRepo)
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
    }

    private static async Task<IResult> DeleteAnimal(
        int id,
        IAnimalRepository repo)
    {
        await repo.DeleteAsync(id);
        return Results.NoContent();
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