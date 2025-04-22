namespace Presentation.DTOs;

public record FeedingScheduleDto(
    int AnimalId,
    string Time,
    string FoodType)
{
    public string Time { get; init; } = Time;

    public string FoodType { get; init; } = FoodType;
}