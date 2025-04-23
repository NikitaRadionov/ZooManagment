using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTOs;

namespace Presentation.Controllers;


public static class FoodTypeHelper
{
    public static FoodType GetFoodType(string foodName)
    {
        try
        {
            return FoodType.FromString(foodName);
        }
        catch (ArgumentException)
        {
            return FoodType.GetRandom();
        }
    }
}

[ApiController]
[Route("api/[controller]")]
public class FeedingScheduleController : ControllerBase
{
    private readonly IFeedingScheduleRepository _feedingScheduleRepo;
    private readonly IAnimalRepository _animalRepo;
    private readonly IDomainEventDispatcher _dispatcher;

    public FeedingScheduleController(
        IFeedingScheduleRepository feedingScheduleRepo,
        IAnimalRepository animalRepo,
        IDomainEventDispatcher dispatcher)
    {
        _feedingScheduleRepo = feedingScheduleRepo;
        _animalRepo = animalRepo;
        _dispatcher = dispatcher;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var schedules = await _feedingScheduleRepo.GetAllAsync();
        return Ok(schedules);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] FeedingScheduleDto dto)
    {
        var animal = await _animalRepo.GetByIdAsync(dto.AnimalId);
        if (animal is null)
            return NotFound("Animal not found");

        TimeOnly time;
        try
        {
            time = TimeOnly.Parse(dto.Time);
        }
        catch
        {
            time = TimeOnly.FromDateTime(DateTime.Now);
        }

        var foodType = FoodTypeHelper.GetFoodType(dto.FoodType);

        var schedule = new FeedingSchedule(animal, new FeedingTime(time), foodType, _dispatcher);
        await _feedingScheduleRepo.AddAsync(schedule);

        return CreatedAtAction(nameof(GetAll), new { id = schedule.Id }, new
        {
            schedule.Id,
            Time = time.ToString("HH:mm"),
            FoodType = foodType.Name,
            Animal = animal.Name
        });
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteFeeding(Guid id)
    {
        var schedule = await _feedingScheduleRepo.GetByIdAsync(id);
        if (schedule is null)
            return NotFound("Feeding schedule not found");

        schedule.MarkAsCompleted();
        await _feedingScheduleRepo.UpdateAsync(schedule);

        return Ok(new { Message = "Feeding completed" });
    }
}

