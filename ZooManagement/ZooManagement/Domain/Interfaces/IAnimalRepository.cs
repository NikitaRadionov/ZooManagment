using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAnimalRepository
    {
        Task<Animal> GetByIdAsync(int id);
        Task<List<Animal>> GetAllAsync();
        Task AddAsync(Animal animal);
        Task UpdateAsync(Animal animal);
        Task DeleteAsync(int id);


    }
}

