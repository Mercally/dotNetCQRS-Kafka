using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infraestructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infraestructure.Handlers;

public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore;

    public EventSourcingHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        PostAggregate aggregate = new PostAggregate();
        List<BaseEvent> events = await _eventStore.GetEventAsync(aggregateId);

        if (events == null || !events.Any())
        {
            return aggregate;
        }

        aggregate.ReplyEvents(events);

        aggregate.Version = events.Select(x => x.Version).Max(); // setting lastet version

        return aggregate;
    }

    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await _eventStore.SaveEventAsync(aggregate.Id, aggregate.GetUncommitedChanges(), aggregate.Version);

        aggregate.MarkChangesAsCommited();
    }
}
