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

builder.Services.AddSimulatorServices(builder.Configuration);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapSimulatorEndpoint();
app.Run();