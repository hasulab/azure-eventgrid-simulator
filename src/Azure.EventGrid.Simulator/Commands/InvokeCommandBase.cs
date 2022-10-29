using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using MediatR;

namespace Azure.EventGrid.Simulator.Commands;

public class InvokeCommandBase : IRequest
{
    public SubscriptionSettings Subscription { get; }
    public EventGridEvent EventGridEvent { get; }
    public string TopicName { get; }

    public InvokeCommandBase(SubscriptionSettings subscription, EventGridEvent eventGridEvent, string topicName)
    {
        Subscription = subscription;
        EventGridEvent = eventGridEvent;
        TopicName = topicName;
    }
}