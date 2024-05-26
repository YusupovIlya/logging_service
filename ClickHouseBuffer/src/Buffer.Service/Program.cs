using Buffer.Service.Handlers;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Monq.Core.ClickHouseBuffer;
using Monq.Core.ClickHouseBuffer.DependencyInjection;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

Environment.SetEnvironmentVariable("WriteToDb", false.ToString());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureCHBuffer(builder.Configuration.GetSection("BufferEngineOptions"), "Host=clickhouse;Port=8123;Username=someuser;Password=strongPasw;Database=logging_service;");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<LogsConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("logs", e =>
        {
            e.ConfigureConsumer<LogsConsumer>(context);
        });
    });
});

var app = builder.Build();

app.MapPost("/api/engine-options", async (HttpContext context, IOptionsMonitor<EngineOptions> options, IConfiguration configuration) =>
{
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    var optionsData = JsonSerializer.Deserialize<EngineOptions>(requestBody);

    options.CurrentValue.EventsFlushCount = optionsData.EventsFlushCount;
    options.CurrentValue.MaxDegreeOfParallelism = optionsData.MaxDegreeOfParallelism;
    configuration.Bind("EngineOptions", options.CurrentValue);

    await context.Response.WriteAsync("Engine options updated successfully");
});

app.MapGet("/api/engine-options", async (HttpContext context, IOptionsMonitor<EngineOptions> options) =>
{
    var currentOptions = options.CurrentValue;
    var responseBody = JsonSerializer.Serialize(currentOptions);

    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(responseBody);
});

app.MapGet("/api/writeToDb", ([FromQuery] bool val, IConfiguration configuration) =>
{
    Environment.SetEnvironmentVariable("WriteToDb", val.ToString());
    return Results.Ok();
});

app.MapGet("/api/writeToDbVal", async (HttpContext context, IConfiguration configuration) =>
{
    await context.Response.WriteAsync(Environment.GetEnvironmentVariable("WriteToDb"));
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
