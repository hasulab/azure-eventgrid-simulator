using Azure.EventGrid.Simulator.Extensions;
using Azure.EventGrid.Simulator.Services;
using MediatR;
using Newtonsoft.Json;

namespace Azure.EventGrid.Simulator.Commands;

public class SendEventsToSubscriberCommandHandler : IRequestHandler<SendEventsToSubscriberCommand>
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
    public Task Handle(SendEventsToSubscriberCommand request, CancellationToken cancellationToken)
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
                            switch (subscription?.Destination?.EndpointType)
                            {
                                case Constants.EndPointTypeWebHook:
                                    _queueService.Enqueue(new InvokeWebHookCommand(subscription, eventGridEvent, topicName));
                                    break;
                                case Constants.EndPointTypeBlob:
                                    _queueService.Enqueue(new InvokeStorageBlobCommand(subscription, eventGridEvent, topicName));
                                    break;
                                case Constants.EndPointTypeQueue:
                                    _queueService.Enqueue(new InvokeStorageQueueCommand(subscription, eventGridEvent, topicName));
                                    break;
                                default:
                                    _logger.LogError("Event {EventId} not valid type for subscriber '{SubscriberName}'", @eventGridEvent.Id,
                                        subscription.Name);
                                    break;
                            }
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