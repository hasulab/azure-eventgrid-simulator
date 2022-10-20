using Azure.EventGrid.Simulator.Extensions;
using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using MediatR;
using Newtonsoft.Json;
using System.Reflection;
using Azure.EventGrid.Simulator.Commands;
using Microsoft.Extensions.DependencyInjection;
using Azure.EventGrid.Simulator.Services;

var builder = WebApplication.CreateBuilder(args);

var settings = new SimulatorSettings();
builder.Configuration.Bind(settings);

builder.Services.AddSingleton(_ => settings);
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddHttpClient();
builder.Services.Configure<EventDeliverySettings>(opt
    => builder.Configuration.GetSection("EventDeliverySettings")
        .Bind(opt));
builder.Services.AddHostedService<EventDeliveryService>();

builder.Services.AddSingleton<IEventQueueService, EventQueueService>();

var app = builder.Build();


app.MapGet("/", () => "Hello World!");
app.MapPost("/api/events", 
    async (HttpContext context, IMediator mediator, SimulatorSettings settings) =>
{
    var requestBody = await context.RequestBody();
    var topic = settings.Topics.First(x => x.Port == context.Request.Host.Port);
    var events = JsonConvert.DeserializeObject<EventGridEvent[]>(requestBody);
    var resposne = await mediator.Send(new SendEventsToSubscriberCommand(events, topic));
});
app.Run();