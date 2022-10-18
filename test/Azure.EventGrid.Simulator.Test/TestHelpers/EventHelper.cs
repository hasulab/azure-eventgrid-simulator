using System.Text.Json;
using Azure.Core.Serialization;
using Azure.Messaging.EventGrid;

namespace Azure.EventGrid.Simulator.Tests.TestHelpers
{
    public static class EventHelper
    {
        public static EventGridEvent GetEventGridEvent(object data) => new(data.GetType().FullName,
            data.GetType().FullName, data.GetType().AssemblyQualifiedName, data);

        public static EventGridEvent GetValidationHandshakeEvent() =>
            new("Microsoft.EventGrid.SubscriptionValidationEvent",
                "Microsoft.EventGrid.SubscriptionValidationEvent",
                string.Empty,
                new
                {
                    validationCode = "1294817205981725"
                });

        public static readonly JsonObjectSerializer Serializer = new(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
