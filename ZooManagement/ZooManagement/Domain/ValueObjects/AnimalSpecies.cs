namespace Domain.ValueObjects
{
    public sealed record AnimalSpecies
    {
        public string Value { get; }

        private AnimalSpecies(string value) => Value = value;

        public static AnimalSpecies Create(string species)
        {
            if (string.IsNullOrWhiteSpace(species))
                throw new ArgumentException("Species cannot be empty");

            return new AnimalSpecies(species.Trim());
        }
    }
}