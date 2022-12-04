using Azure.EventGrid.Simulator.Commands;
using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using MediatR;
using Newtonsoft.Json;

namespace Azure.EventGrid.Simulator.Extensions
{
    public static class EndpointRouteBuilderExtensions
	{
		public static void MapSimulatorEndpoint(this IEndpointRouteBuilder endpoint)
		{
			endpoint.MapPost("/api/events",
				async (HttpContext context, IMediator mediator, SimulatorSettings settings) =>
				{
					var requestBody = await context.RequestBody();
					var topic = settings.Topics.First(x => x.Port == context.Request.Host.Port);
					var events = JsonConvert.DeserializeObject<EventGridEvent[]>(requestBody);
					var resposne = await mediator.Send(new SendEventsToSubscriberCommand(events, topic));
				});
		}
	}
}
