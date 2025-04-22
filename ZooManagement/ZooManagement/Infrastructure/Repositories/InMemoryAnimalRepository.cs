
using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Concurrent;

namespace Infrastructure.Repositories
{
    public class InMemoryAnimalRepository : IAnimalRepository
    {
        private readonly ConcurrentDictionary<int, Animal> _animals = new();
        private int _nextId = 0;

        public Task<Animal> GetByIdAsync(int id)
            => Task.FromResult(_animals.TryGetValue(id, out var animal) ? animal : null);

        public Task<List<Animal>> GetAllAsync()
            => Task.FromResult(_animals.Values.ToList());

        public Task AddAsync(Animal animal)
        {
            animal.Id = _nextId++;
            _animals.TryAdd(animal.Id, animal);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Animal animal)
        {
            _animals.TryUpdate(animal.Id, animal, _animals[animal.Id]);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            _animals.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }

}