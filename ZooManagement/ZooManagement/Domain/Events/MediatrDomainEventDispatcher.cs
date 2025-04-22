using Domain.Interfaces;

namespace Domain.Events
{
    public class MediatrDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public MediatrDomainEventDispatcher(IMediator mediator)
            => _mediator = mediator;

        public async Task Dispatch(DomainEvent domainEvent)
            => await _mediator.Publish(domainEvent);
    }
}