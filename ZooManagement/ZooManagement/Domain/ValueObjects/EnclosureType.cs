
namespace Domain.ValueObjects;


public sealed record EnclosureType
{
    public static EnclosureType Predator = new("Predator");
    public static EnclosureType Herbivore = new("Herbivore");
    public static EnclosureType Birds = new("Birds");
    public static EnclosureType Aquarium = new("Aquarium");

    private static readonly List<EnclosureType> _allTypes = new()
    {
        Predator, Herbivore, Birds, Aquarium
    };

    private static readonly Random _random = new();

    public string Name { get; }

    private EnclosureType(string name) => Name = name;

    public static EnclosureType FromString(string typeName)
    {
        return typeName.ToLower() switch
        {
            "predator" => Predator,
            "herbivore" => Herbivore,
            "birds" => Birds,
            "aquarium" => Aquarium,
            _ => GetRandomType()
        };
    }

    public static EnclosureType GetRandomType() =>
        _allTypes[_random.Next(_allTypes.Count)];

    public static List<string> GetValidTypes() =>
        _allTypes.Select(t => t.Name).ToList();
}
