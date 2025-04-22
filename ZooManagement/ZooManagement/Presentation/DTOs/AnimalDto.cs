using System.ComponentModel.DataAnnotations;

namespace Presentation.DTOs;


public record AnimalDto(
    [Required] string Species,
    [Required] string Name,
    [Required] string BirthDate,
    [Required] string Gender,
    [Required] string FavoriteFood,
    bool IsHealthy);


