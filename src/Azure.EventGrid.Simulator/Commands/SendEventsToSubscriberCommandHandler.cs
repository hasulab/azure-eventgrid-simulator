﻿using Azure.EventGrid.Simulator.Extensions;
using Azure.EventGrid.Simulator.Services;
using MediatR;
using Newtonsoft.Json;

namespace Azure.EventGrid.Simulator.Commands;

public class SendEventsToSubscriberCommandHandler : AsyncRequestHandler<SendEventsToSubscriberCommand>
{
    private readonly ILogger<SendEventsToSubscriberCommandHandler> _logger;
    private readonly IEventQueueService _queueService;
    private readonly IMediator _mediator;
    private const string _validationHandshakeEventType = "Microsoft.EventGrid.SubscriptionValidationEvent";

    public SendEventsToSubscriberCommandHandler(ILogger<SendEventsToSubscriberCommandHandler> logger,
        IEventQueueService queueService, IMediator mediator)
    {
        _logger = logger;
        _queueService = queueService;
        _mediator = mediator;
    }
    protected override Task Handle(SendEventsToSubscriberCommand request, CancellationToken cancellationToken)
    {
        string topicName = request.Topic.Name;
        foreach (var @eventGridEvent in request.Events)
        {
            try
            {
                if (@eventGridEvent.EventType == _validationHandshakeEventType)
                {
                    _logger.LogInformation($"{nameof(SendEventsToSubscriberCommandHandler)} is processing {_validationHandshakeEventType}");

                    dynamic data = JsonConvert.DeserializeObject(eventGridEvent.Data.ToString() ?? string.Empty);

                    if (data?.validationCode != null)
                    {
                        _logger.LogInformation($"{nameof(SendEventsToSubscriberCommandHandler)} responds with validation code for {_validationHandshakeEventType}");

                        dynamic result = new { validationResponse = data.validationCode.ToString() };

                        //return result;
                    }
                }
                else
                {
                    foreach (var subscription in request.Topic.Subscribers)
                    {
                        if (subscription.Filter.AcceptsEvent(@eventGridEvent))
                        {
                            _queueService.Enqueue(new InvokeWebHookCommand(subscription, eventGridEvent, topicName));
                        }
                        else
                        {
                            _logger.LogDebug("Event {EventId} filtered out for subscriber '{SubscriberName}'", @eventGridEvent.Id,
                                subscription.Name);
                        }
                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        return Task.CompletedTask;
    }

}