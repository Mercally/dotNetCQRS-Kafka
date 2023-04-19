using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infraestructure.Converters;
using Post.Query.Infraestructure.Handlers;

namespace Post.Query.Infraestructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly IEventHandler _eventHandler;

    public EventConsumer(IOptions<ConsumerConfig> consumerConfig, IEventHandler eventHandler)
    {
        _eventHandler = eventHandler;
        _consumerConfig = consumerConfig.Value;
    }

    public void Consume(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(_consumerConfig)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        consumer.Subscribe(topic);

        while (true)
        {
            var consumerResult = consumer.Consume();

            if (consumerResult?.Message == null)
            {
                continue;
            }

            var jsonOptions = new JsonSerializerOptions()
            {
                Converters = { new EventJsonConverter() }
            };

            BaseEvent @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, jsonOptions)
                ?? new BaseEvent.EmptyEvent();

            if (string.IsNullOrWhiteSpace(@event.Type))
            {
                continue;
            }

            MethodInfo? handlerMethod = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });

            if (handlerMethod == null)
            {
                throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method.");
            }

            handlerMethod.Invoke(_eventHandler, new object[] { @event });

            consumer.Commit(consumerResult);
        }
    }
}
