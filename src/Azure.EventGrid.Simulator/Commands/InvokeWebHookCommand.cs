﻿using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using MediatR;
using System.Net.Http;

namespace Azure.EventGrid.Simulator.Commands;

public class InvokeWebHookCommand : InvokeCommandBase, IRequest
{
    public InvokeWebHookCommand(SubscriptionSettings subscription, EventGridEvent eventGridEvent, string topicName)
        : base(subscription, eventGridEvent, topicName)
    {
    }
}