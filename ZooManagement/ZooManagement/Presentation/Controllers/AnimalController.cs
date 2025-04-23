using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTOs;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalController : ControllerBase
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IEnclosureRepository _enclosureRepository;
    private readonly ZooStatisticsService _statsService;
    private readonly AnimalTransferService _transferService;
    private readonly IDomainEventDispatcher _dispatcher;

    public AnimalController(
        IAnimalRepository animalRepository,
        IEnclosureRepository enclosureRepository,
        ZooStatisticsService statsService,
        AnimalTransferService transferService,
        IDomainEventDispatcher dispatcher)
    {
        _animalRepository = animalRepository;
        _enclosureRepository = enclosureRepository;
        _statsService = statsService;
        _transferService = transferService;
        _dispatcher = dispatcher;
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetZooStatistics()
    {
        var stats = await _statsService.GetStatisticsAsync();
        return Ok(stats);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAnimals()
    {
        var animals = await _animalRepository.GetAllAsync();
        return Ok(animals);
    }

    [HttpPost]
    public async Task<IActionResult> AddAnimal([FromBody] AnimalDto dto)
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
                _dispatcher);

            animal.Heal();
            await _animalRepository.AddAsync(animal);

            return CreatedAtAction(nameof(GetAllAnimals), new { id = animal.Id }, new
            {
                animal.Id,
                dto.Species,
                AssignedBirthDate = birthDate.ToString("yyyy-MM-dd"),
                AssignedFoodType = foodType.Name
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferAnimal([FromBody] TransferRequestDto request)
    {
        var animal = await _animalRepository.GetByIdAsync(request.AnimalId);
        var enclosure = await _enclosureRepository.GetByIdAsync(request.NewEnclosureId);

        if (animal == null || enclosure == null)
        {
            var errors = new List<string>();
            if (animal == null) errors.Add($"Animal with ID {request.AnimalId} not found");
            if (enclosure == null) errors.Add($"Enclosure with ID {request.NewEnclosureId} not found");
            return BadRequest(new { Errors = errors });
        }

        try
        {
            await _transferService.TransferAnimalAsync(animal, enclosure);
            return Ok(new { Message = "Transfer completed successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnimal(int id)
    {
        await _animalRepository.DeleteAsync(id);
        return NoContent();
    }

    private FoodType GetFoodType(string foodName)
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
