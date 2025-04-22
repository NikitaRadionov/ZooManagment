using System.ComponentModel.DataAnnotations;

namespace Presentation.DTOs;

public record TransferRequestDto(
    [Range(0, int.MaxValue, ErrorMessage = "Animal ID must be non-negative")]
    int AnimalId,

    [Range(0, int.MaxValue, ErrorMessage = "Enclosure ID must be non-negative")]
    int NewEnclosureId
);