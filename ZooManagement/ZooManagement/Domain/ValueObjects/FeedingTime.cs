namespace Domain.ValueObjects
{
    public sealed record FeedingTime
    {
        public TimeOnly Time { get; }

        public FeedingTime(TimeOnly time)
        {
            if (time < TimeOnly.MinValue || time > TimeOnly.MaxValue)
                throw new ArgumentException("Invalid feeding time");

            Time = time;
        }
        internal static FeedingTime Create(string time)
        => new(TimeOnly.Parse(time));

    }
}