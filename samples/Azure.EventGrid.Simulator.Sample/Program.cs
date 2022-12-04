using Azure.EventGrid.Simulator.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSimulatorServices(builder.Configuration);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapSimulatorEndpoint();
app.Run();