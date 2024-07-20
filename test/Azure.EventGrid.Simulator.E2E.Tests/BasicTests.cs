using Azure.EventGrid.Simulator.Extensions;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace Azure.EventGrid.Simulator.E2E.Tests
{
    public class BasicTests
    : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public BasicTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            EndpointRouteBuilderExtensions.IsE2ETestCall = true;
        }

        [Theory]
        [InlineData("/api/events/")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var @event = new EventGridEvent("test subject", "test type", "1.0", new { id = 1 });

            EventGridEvent[] events = [@event];

            // Act
            var response = await client.PostAsync(url, JsonContent.Create(events));

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }
    }
}