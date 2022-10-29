using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using MediatR;

namespace Azure.EventGrid.Simulator.Commands;

public class InvokeStorageQueueCommand : InvokeCommandBase, IRequest
{
    public InvokeStorageQueueCommand(SubscriptionSettings subscription, EventGridEvent eventGridEvent, string topicName)
        : base(subscription, eventGridEvent, topicName)
    {
    }
}