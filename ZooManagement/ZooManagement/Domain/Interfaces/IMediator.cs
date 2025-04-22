
using Domain.Events;
namespace Domain.Interfaces;
public interface IMediator
{
    Task Publish(DomainEvent domainEvent);
}