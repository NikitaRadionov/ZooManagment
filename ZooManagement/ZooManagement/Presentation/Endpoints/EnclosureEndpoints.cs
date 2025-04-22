using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Presentation.DTOs;

public static class EnclosureEndpoints
{

    public static void MapEnclosureEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/enclosures")
            .WithTags("Enclosures");

        group.MapPost("/", async (
        [FromBody] EnclosureDto dto,
        IEnclosureRepository repo) =>
        {
            var validationErrors = new List<string>();

            if (dto.Size <= 0) validationErrors.Add("Size must be greater than 0");
            if (dto.MaxCapacity <= 0) validationErrors.Add("MaxCapacity must be greater than 0");

            if (validationErrors.Any())
                return Results.BadRequest(new { Errors = validationErrors });

            var enclosureType = EnclosureType.FromString(dto.Type);

            var enclosure = new Enclosure(
                enclosureType,
                dto.Size,
                dto.MaxCapacity);

            await repo.AddAsync(enclosure);

            return Results.Created($"/enclosures/{enclosure.Id}", new
            {
                enclosure.Id,
                AssignedType = enclosureType.Name,
                dto.Size,
                dto.MaxCapacity
            });
        })
        .WithName("AddEnclosure")
        .Accepts<EnclosureDto>("application/json")
        .Produces(201)
        .Produces(400);






    }

}