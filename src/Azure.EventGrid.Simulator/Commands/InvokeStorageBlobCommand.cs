using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using MediatR;

namespace Azure.EventGrid.Simulator.Commands;

public class InvokeStorageBlobCommand : InvokeCommandBase, IRequest
{
    public InvokeStorageBlobCommand(SubscriptionSettings subscription, EventGridEvent eventGridEvent, string topicName)
        : base(subscription, eventGridEvent, topicName)
    {
    }
}