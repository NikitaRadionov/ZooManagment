
using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Concurrent;

namespace Infrastructure.Repositories
{
    public class InMemoryFeedingScheduleRepository : IFeedingScheduleRepository
    {
        private static readonly ConcurrentDictionary<Guid, FeedingSchedule> _schedules = new();

        public Task<FeedingSchedule> GetByIdAsync(Guid id)
            => Task.FromResult(_schedules.GetValueOrDefault(id));

        public Task AddAsync(FeedingSchedule schedule)
        {
            _schedules.TryAdd(schedule.Id, schedule);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(FeedingSchedule schedule)
        {
            _schedules.TryUpdate(schedule.Id, schedule, _schedules[schedule.Id]);
            return Task.CompletedTask;
        }

        public Task<List<FeedingSchedule>> GetAllAsync()
    => Task.FromResult(_schedules.Values.ToList());
    }
}