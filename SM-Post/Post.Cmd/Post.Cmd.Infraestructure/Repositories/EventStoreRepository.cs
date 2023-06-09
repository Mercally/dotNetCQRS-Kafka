using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infraestructure.Config;

namespace Post.Cmd.Infraestructure.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection;

    public EventStoreRepository(IOptions<MongoDbConfig> config)
    {
        MongoClient mongoClient = new MongoClient(config.Value.ConnectionString);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(config.Value.Database);

        _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
    }

    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventStoreCollection
            .Find(x => x.AggregateIndentifier == aggregateId)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task SaveAsync(EventModel @event)
    {
        await _eventStoreCollection
            .InsertOneAsync(@event)
            .ConfigureAwait(false);
    }
}
