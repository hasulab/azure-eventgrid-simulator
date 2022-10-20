using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using MediatR;
using System.Net.Http;

namespace Azure.EventGrid.Simulator.Commands;

public class InvokeWebHookCommand : IRequest
{
    public SubscriptionSettings Subscription { get; }
    public EventGridEvent EventGridEvent { get; }
    public string TopicName { get; }

    public InvokeWebHookCommand(SubscriptionSettings subscription, EventGridEvent eventGridEvent, string topicName)
    {
        Subscription = subscription;
        EventGridEvent = eventGridEvent;
        TopicName = topicName;
    }
}