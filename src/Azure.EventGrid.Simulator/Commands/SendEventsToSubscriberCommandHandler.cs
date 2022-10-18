using MediatR;
using Newtonsoft.Json;

namespace Azure.EventGrid.Simulator.Commands;

public class SendEventsToSubscriberCommandHandler : AsyncRequestHandler<SendEventsToSubscriberCommand>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SendEventsToSubscriberCommandHandler> _logger;
    private const string _validationHandshakeEventType = "Microsoft.EventGrid.SubscriptionValidationEvent";

    public SendEventsToSubscriberCommandHandler(IHttpClientFactory httpClientFactory, ILogger<SendEventsToSubscriberCommandHandler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
    protected override async Task Handle(SendEventsToSubscriberCommand request, CancellationToken cancellationToken)
    {
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        throw new NotImplementedException();
    }
}