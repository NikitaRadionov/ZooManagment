using System.ComponentModel.DataAnnotations;

namespace Presentation.DTOs;
public record EnclosureDto(
    string Type,
    [Range(0.1, double.MaxValue)] double Size,
    [Range(1, int.MaxValue)] int MaxCapacity)
{
    public string Type { get; init; } = Type;
}
