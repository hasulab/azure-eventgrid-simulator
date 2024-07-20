using Azure.EventGrid.Simulator.Commands;
using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using MediatR;
using Newtonsoft.Json;

namespace Azure.EventGrid.Simulator.Extensions
{
    public static class EndpointRouteBuilderExtensions
	{
		static internal bool IsE2ETestCall = false;
		public static void MapSimulatorEndpoint(this IEndpointRouteBuilder endpoint)
		{
			endpoint.MapPost("/api/events",
				async (HttpContext context, IMediator mediator, SimulatorSettings settings) =>
				{
					var requestBody = await context.RequestBody();
					var port = IsE2ETestCall ? 5002 : context.Request.Host.Port;

					var topic = settings.Topics.First(x => x.Port == port);
					var events = JsonConvert.DeserializeObject<EventGridEvent[]>(requestBody);
					await mediator.Send(new SendEventsToSubscriberCommand(events, topic));
				});
		}
	}
}
