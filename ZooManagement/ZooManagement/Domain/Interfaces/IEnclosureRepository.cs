using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEnclosureRepository
    {
        Task<Enclosure> GetByIdAsync(int id);
        Task<List<Enclosure>> GetAllAsync();
        Task AddAsync(Enclosure enclosure);
        Task UpdateAsync(Enclosure enclosure);
        Task DeleteAsync(int id);

    }
}
