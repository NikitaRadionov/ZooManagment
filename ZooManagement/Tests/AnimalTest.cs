using Domain.Entities;
using Domain.ValueObjects;
using Domain.Events;
using Xunit;

public class AnimalTests
{
    [Fact]
    public void Animal_Created_WithCorrectProperties()
    {
        var dispatcher = new FakeDispatcher();
        var species = AnimalSpecies.Create("Lion");
        var foodType = FoodType.Meat;

        var animal = new Animal(species, "Simba", DateOnly.Parse("2020-01-15"), "Male", foodType, dispatcher);

        Assert.Equal("Lion", animal.Species.Value);
        Assert.Equal("Simba", animal.Name);
        Assert.True(animal.IsHealthy);
    }

    [Fact]
    public void Heal_SetsIsHealthyToTrue()
    {
        var dispatcher = new FakeDispatcher();
        var animal = new Animal(
            AnimalSpecies.Create("Tiger"),
            "Max",
            DateOnly.Parse("2019-05-01"),
            "Female",
            FoodType.Meat,
            dispatcher
        );

        animal.Heal();

        Assert.True(animal.IsHealthy);
    }
}