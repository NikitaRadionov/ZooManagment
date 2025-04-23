using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTOs;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnclosureController : ControllerBase
{
    private readonly IEnclosureRepository _enclosureRepository;

    public EnclosureController(IEnclosureRepository enclosureRepository)
    {
        _enclosureRepository = enclosureRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEnclosures()
    {
        var enclosures = await _enclosureRepository.GetAllAsync();
        return Ok(enclosures);
    }

    [HttpPost]
    public async Task<IActionResult> AddEnclosure([FromBody] EnclosureDto dto)
    {
        var enclosureType = GetEnclosureType(dto.Type);
        var enclosure = new Enclosure(
            enclosureType,
            dto.Size,
            dto.MaxCapacity);

        await _enclosureRepository.AddAsync(enclosure);
        return CreatedAtAction(nameof(GetAllEnclosures), new { id = enclosure.Id }, enclosure);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEnclosure(int id)
    {
        await _enclosureRepository.DeleteAsync(id);
        return NoContent();
    }

    private EnclosureType GetEnclosureType(string typeName)
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
