
namespace Domain.ValueObjects;



public sealed record FoodType
{
    public static FoodType Meat = new("Meat");
    public static FoodType Vegetables = new("Vegetables");
    public static FoodType Fish = new("Fish");
    public static FoodType Grains = new("Grains");

    private static readonly List<FoodType> _allFoodTypes = new()
    {
        Meat,
        Vegetables,
        Fish,
        Grains
    };

    private static readonly Random _random = new();

    public string Name { get; }

    private FoodType(string name) => Name = name;

    public static FoodType FromString(string foodName)
    {
        return foodName.ToLower() switch
        {
            "meat" => Meat,
            "vegetables" => Vegetables,
            "fish" => Fish,
            "grains" => Grains,
            _ => throw new ArgumentException($"Invalid food type: {foodName}")
        };
    }

    public static FoodType GetRandom() =>
        _allFoodTypes[_random.Next(_allFoodTypes.Count)];

    public static List<string> GetValidTypes() =>
        _allFoodTypes.Select(ft => ft.Name).ToList();
}
