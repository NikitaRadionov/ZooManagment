using Domain.Events;
namespace Domain.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task Dispatch(DomainEvent domainEvent);
    }
}