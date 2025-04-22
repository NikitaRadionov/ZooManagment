
using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Concurrent;





namespace Infrastructure.Repositories
{
    public class InMemoryEnclosureRepository : IEnclosureRepository
    {
        private readonly ConcurrentDictionary<int, Enclosure> _enclosures = new();
        private int _nextId = 0;

        public Task<Enclosure> GetByIdAsync(int id)
            => Task.FromResult(_enclosures.GetValueOrDefault(id));

        public Task<List<Enclosure>> GetAllAsync()
            => Task.FromResult(_enclosures.Values.ToList());

        public Task AddAsync(Enclosure enclosure)
        {
            enclosure.Id = _nextId++;
            _enclosures.TryAdd(enclosure.Id, enclosure);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Enclosure enclosure)
        {
            _enclosures.TryUpdate(enclosure.Id, enclosure, _enclosures[enclosure.Id]);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            _enclosures.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}