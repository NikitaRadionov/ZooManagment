using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTOs;

namespace Presentation.Endpoints;

public static class EnclosureEndpoints
{
    public static RouteGroupBuilder MapEnclosureApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IEnclosureRepository repo) =>
            Results.Ok(await repo.GetAllAsync()))
            .WithName("GetAllEnclosures");

        group.MapPost("/", AddEnclosure)
            .WithName("AddEnclosure")
            .Accepts<EnclosureDto>("application/json")
            .Produces<Enclosure>(201);

        group.MapDelete("/{id}", DeleteEnclosure)
            .WithName("DeleteEnclosure")
            .Produces(204)
            .Produces(404);

        return group;
    }

    private static async Task<IResult> AddEnclosure(
        [FromBody] EnclosureDto dto,
        IEnclosureRepository repo)
    {
        var enclosureType = GetEnclosureType(dto.Type);
        var enclosure = new Enclosure(
            enclosureType,
            dto.Size,
            dto.MaxCapacity);

        await repo.AddAsync(enclosure);
        return Results.Created($"/enclosures/{enclosure.Id}", enclosure);
    }

    private static async Task<IResult> DeleteEnclosure(
        int id,
        IEnclosureRepository repo)
    {
        await repo.DeleteAsync(id);
        return Results.NoContent();
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
}