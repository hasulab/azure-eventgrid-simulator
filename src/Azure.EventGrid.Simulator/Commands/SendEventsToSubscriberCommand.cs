using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using MediatR;

namespace Azure.EventGrid.Simulator.Commands
{
    public class SendEventsToSubscriberCommand: IRequest
    {
        public SendEventsToSubscriberCommand(EventGridEvent[] events, TopicSettings topic)
        {
            Events = events;
            Topic = topic;
        }

        public TopicSettings Topic { get; }

        public EventGridEvent[] Events { get; }
    }
}
