using Azure.EventGrid.Simulator.E2E.Tests.TestHelpers;
using Azure.Messaging.EventGrid;

namespace Azure.EventGrid.Simulator.E2E.Tests
{
    public class EventGridEventClientTest
    {
        [Fact]
        public async Task Test1()
        {
            var uri = new Uri("https://localhost:7241/api/events/");
            var client = new EventGridPublisherClient(uri, new AzureKeyCredential("test key"));
            var @event = new EventGridEvent("test subject","test type","1.0", new {id=1});
            var response = await client.SendEventAsync(@event);
        }

        [Fact]
        public async Task TestEvents()
        {
            var uri = new Uri("https://localhost:7241/api/events/");
            var client = new EventGridPublisherClient(uri, new AzureKeyCredential("test key"));
            var @event =EventHelper.GetEventGridEvent(new TestEventData{ Id = 10});
            var response = await client.SendEventAsync(@event);
        }
    }

    public class TestEventData
    {
        public int Id { get; set; } = 1;
    }
}