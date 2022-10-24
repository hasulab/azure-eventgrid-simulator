using System.Text;
using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using MediatR;
using Newtonsoft.Json;

namespace Azure.EventGrid.Simulator.Commands;

public class InvokeWebHookCommandHandler : AsyncRequestHandler<InvokeWebHookCommand>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<InvokeWebHookCommandHandler> _logger;

    public InvokeWebHookCommandHandler(IHttpClientFactory httpClientFactory, ILogger<InvokeWebHookCommandHandler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
    protected override async Task Handle(InvokeWebHookCommand request, CancellationToken cancellationToken)
    {
        await InvokeWebHook(request.Subscription,request.EventGridEvent, request.TopicName, cancellationToken);
    }

    private async Task InvokeWebHook(SubscriptionSettings subscription, EventGridEvent eventGridEvent, string topicName
        , CancellationToken cancellationToken)
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

        var endpoint = subscription.Destination.Properties.Endpoint;

        await httpClient.PostAsync(endpoint, content, cancellationToken)
            .ContinueWith(t => LogResult(t, @eventGridEvent, subscription, topicName), cancellationToken);
    }
    private void LogResult(Task<HttpResponseMessage> task, EventGridEvent evt, SubscriptionSettings subscription, string topicName)
    {
        if (task.IsCompletedSuccessfully && task.Result.IsSuccessStatusCode)
        {
            _logger.LogDebug("Event {EventId} sent to subscriber '{SubscriberName}' on topic '{TopicName}' successfully", evt.Id, subscription.Name, topicName);
        }
        else
        {
            try
            {
                _logger.LogError(task.Exception,
                    "Failed to send event {EventId} to subscriber '{SubscriberName}', '{TaskStatus}', '{Reason}'",
                    evt.Id,
                    subscription.Name,
                    task.Status.ToString(),
                    task.Result?.ReasonPhrase);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

}