
using Domain.Entities;


namespace Domain.Interfaces;

public interface IFeedingScheduleRepository
{
    Task<FeedingSchedule> GetByIdAsync(Guid id);
    Task<List<FeedingSchedule>> GetAllAsync();
    Task AddAsync(FeedingSchedule schedule);
    Task UpdateAsync(FeedingSchedule schedule);

}