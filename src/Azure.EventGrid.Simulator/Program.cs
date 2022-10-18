using Azure.EventGrid.Simulator.Extensions;
using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using MediatR;
using Newtonsoft.Json;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var settings = new SimulatorSettings();
builder.Configuration.Bind(settings);

builder.Services.AddSingleton(_ => settings);
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddHttpClient();

var app = builder.Build();


app.MapGet("/", () => "Hello World!");
app.MapPost("/api/events", 
    async (HttpContext context, IMediator mediator, SimulatorSettings settings) =>
{
    var requestBody = await context.RequestBody();
    var events = JsonConvert.DeserializeObject<EventGridEvent[]>(requestBody);

});
app.Run();