
using Monq.Core.ClickHouseBuffer;
using MassTransit;
using Buffer.Service.Models;
using System.Text.Json;

namespace Buffer.Service.Handlers;

public class LogsConsumer : IConsumer<LogEntity>
{
    readonly IEventsBufferEngine _eventsBufferEngine;
    readonly ILogger<LogsConsumer> _logger;

    public LogsConsumer(
        IEventsBufferEngine eventsBufferEngine,
        ILogger<LogsConsumer> logger)
    {
        _eventsBufferEngine = eventsBufferEngine;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<LogEntity> context)
    {
        if (context.Message is null)
            return Task.CompletedTask;

        try
        {
            var logDb = new LogDb
            {
                Id = Guid.NewGuid(),
                UserId = context.Message.user_id,
                Action = context.Message.action_description,
                Ip = context.Message.ip,
                Endpoint = context.Message.endpoint,
                RawJson = JsonSerializer.Serialize(context.Message),
                TimeStamp = DateTimeOffset.UtcNow,
                Date = DateTime.UtcNow
            };

            return _eventsBufferEngine.AddEvent(logDb, "stream1");
        } catch (Exception ex)
        {
            return Task.CompletedTask;
        }
    }
}