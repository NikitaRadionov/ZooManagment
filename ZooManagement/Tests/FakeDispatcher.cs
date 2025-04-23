using Domain.Events;
using Domain.Interfaces;

public class FakeDispatcher : IDomainEventDispatcher
{
    public Task Dispatch(DomainEvent domainEvent) => Task.CompletedTask;
}