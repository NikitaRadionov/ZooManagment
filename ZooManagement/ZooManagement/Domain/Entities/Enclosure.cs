
using Domain.ValueObjects;


namespace Domain.Entities;
public class Enclosure
{
    public int Id { get; set; }
    public EnclosureType Type { get; private set; }
    public double Size { get; private set; }
    public int MaxCapacity { get; private set; }
    private readonly List<Animal> _animals = new();
    public IReadOnlyList<Animal> Animals => _animals.AsReadOnly();
    public bool NeedsCleaning { get; private set; }

    public Enclosure(EnclosureType type, double size, int maxCapacity)
    {
        Type = type;
        Size = size > 0 ? size : 10;
        MaxCapacity = maxCapacity > 0 ? maxCapacity : 10;
    }

    public void AddAnimal(Animal animal)
    {
        if (_animals.Count >= MaxCapacity)
            throw new InvalidOperationException("The enclosure is overcrowded");

        _animals.Add(animal);
        NeedsCleaning = true;
    }

    public void RemoveAnimal(Animal animal) => _animals.Remove(animal);

    public void Clean()
    {
        NeedsCleaning = false;
        Console.WriteLine($"Enclosure {Type.Name} has been cleaned");
    }
}