using Domain.Entities;
using Domain.Interfaces;
using Application.Services;
using Presentation.DTOs;
using Microsoft.AspNetCore.Mvc;
using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

public static class AnimalEndpoints
{
    public static void MapAnimalEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/animals")
            .WithTags("Animals");

        group.MapGet("/", async (ZooStatisticsService service) =>
            Results.Ok(await service.GetStatisticsAsync()))
            .WithName("GetZooStatistics");

        group.MapGet("/all", async (IAnimalRepository repo) =>
            Results.Ok(await repo.GetAllAsync()))
            .WithName("GetAllAnimals");


        group.MapPost("/transfer", async (
        [FromBody] TransferRequest request,
        AnimalTransferService service,
        IAnimalRepository animalRepo,
        IEnclosureRepository enclosureRepo) =>
        {
            var validationErrors = new List<string>();

            if (request.AnimalId < 0) validationErrors.Add("Animal ID must be non-negative");
            if (request.NewEnclosureId < 0) validationErrors.Add("Enclosure ID must be non-negative");

            if (validationErrors.Any())
                return Results.BadRequest(new { Errors = validationErrors });

            var animal = await animalRepo.GetByIdAsync(request.AnimalId);
            var enclosure = await enclosureRepo.GetByIdAsync(request.NewEnclosureId);

            var errorMessages = new List<string>();
            if (animal == null) errorMessages.Add($"Animal with ID {request.AnimalId} not found");
            if (enclosure == null) errorMessages.Add($"Enclosure with ID {request.NewEnclosureId} not found");

            if (errorMessages.Count > 0)
                return Results.BadRequest(new { Errors = errorMessages });

            try
            {
                await service.TransferAnimalAsync(animal, enclosure);
                return Results.Ok(new
                {
                    Message = "Transfer successful",
                    AnimalId = animal.Id,
                    NewEnclosureId = enclosure.Id
                });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        })
    .WithName("TransferAnimal")
    .Produces(200)
    .Produces<ProblemDetails>(400);



        group.MapPost("/", async (
            [FromBody] AnimalDto dto,
            IAnimalRepository repo,
            IDomainEventDispatcher dispatcher) =>
        {
            try
            {
                var foodType = FoodType.FromString(dto.FavoriteFood);

                if (!DateOnly.TryParse(dto.BirthDate, out var birthDate))
                {
                    return Results.BadRequest("Invalid date format. Use YYYY-MM-DD");
                }

                var animal = new Animal(
                    AnimalSpecies.Create(dto.Species),
                    dto.Name,
                    birthDate,
                    dto.Gender,
                    foodType,
                    dispatcher);

                animal.Heal();
                await repo.AddAsync(animal);
                return Results.Created($"/animals/{animal.Id}", animal);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("Invalid food type"))
            {
                var validTypes = string.Join(", ", FoodType.GetValidTypes());
                return Results.BadRequest($"Invalid food type. Valid values: {validTypes}");
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapDelete("/{id}", async (int id, IAnimalRepository repo) =>
        {
            var animal = await repo.GetByIdAsync(id);
            if (animal is null) return Results.NotFound();

            await repo.DeleteAsync(id);
            return Results.NoContent();
        })
        .WithName("DeleteAnimal")
        .Produces(204)
        .Produces(404);
    }
}