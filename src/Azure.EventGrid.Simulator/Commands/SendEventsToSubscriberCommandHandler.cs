using System.Text;
using Azure.EventGrid.Simulator.Extensions;
using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using MediatR;
using Newtonsoft.Json;

namespace Azure.EventGrid.Simulator.Commands;

public class SendEventsToSubscriberCommandHandler : AsyncRequestHandler<SendEventsToSubscriberCommand>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SendEventsToSubscriberCommandHandler> _logger;
    private const string _validationHandshakeEventType = "Microsoft.EventGrid.SubscriptionValidationEvent";

    public SendEventsToSubscriberCommandHandler(IHttpClientFactory httpClientFactory,
        ILogger<SendEventsToSubscriberCommandHandler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
    protected override async Task Handle(SendEventsToSubscriberCommand request, CancellationToken cancellationToken)
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
                            var json = JsonConvert.SerializeObject(new[] { @eventGridEvent }, Formatting.Indented);
                            using var content = new StringContent(json, Encoding.UTF8, "application/json");
                            var httpClient = _httpClientFactory.CreateClient();
                            httpClient.DefaultRequestHeaders.Add(Constants.AegEventTypeHeader, Constants.NotificationEventType);
                            httpClient.DefaultRequestHeaders.Add(Constants.AegSubscriptionNameHeader, subscription.Name.ToUpperInvariant());
                            httpClient.DefaultRequestHeaders.Add(Constants.AegDataVersionHeader, @eventGridEvent.DataVersion);
                            httpClient.DefaultRequestHeaders.Add(Constants.AegMetadataVersionHeader, @eventGridEvent.MetadataVersion);
                            httpClient.DefaultRequestHeaders.Add(Constants.AegDeliveryCountHeader, "0"); // TODO implement re-tries
                            httpClient.Timeout = TimeSpan.FromSeconds(60);

                            await httpClient.PostAsync(subscription.Endpoint, content)
                                .ContinueWith(t => LogResult(t, @eventGridEvent, subscription, topicName));
                        }
                        else
                        {
                            _logger.LogDebug("Event {EventId} filtered out for subscriber '{SubscriberName}'", @eventGridEvent.Id, subscription.Name);
                        }
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
    private void LogResult(Task<HttpResponseMessage> task, EventGridEvent evt, SubscriptionSettings subscription, string topicName)
    {
        if (task.IsCompletedSuccessfully && task.Result.IsSuccessStatusCode)
        {
            _logger.LogDebug("Event {EventId} sent to subscriber '{SubscriberName}' on topic '{TopicName}' successfully", evt.Id, subscription.Name, topicName);
        }
        else
        {
            _logger.LogError(task.Exception?.GetBaseException(),
                "Failed to send event {EventId} to subscriber '{SubscriberName}', '{TaskStatus}', '{Reason}'",
                evt.Id,
                subscription.Name,
                task.Status.ToString(),
                task.Result?.ReasonPhrase);
        }
    }
}