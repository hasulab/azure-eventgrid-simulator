using Azure.Storage.Queues;
using MediatR;
using Newtonsoft.Json;
using System.Text;

namespace Azure.EventGrid.Simulator.Commands;

public class InvokeStorageQueueCommandHandler : AsyncRequestHandler<InvokeStorageQueueCommand>
{
    private readonly ILogger<InvokeStorageQueueCommandHandler> _logger;

    public InvokeStorageQueueCommandHandler(ILogger<InvokeStorageQueueCommandHandler> logger)
    {
        _logger = logger;

    }

    protected override async Task Handle(InvokeStorageQueueCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending to Queue {QueueName}", request.Subscription.Destination.Properties.QueueName);

        var queueServiceClient = new QueueClient(request.Subscription.Destination.Properties.StorageConnectionString,
            request.Subscription.Destination.Properties.QueueName);
        var jsonData = JsonConvert.SerializeObject(request.EventGridEvent);
        //var fileName = $"{DateTime.UtcNow.ToString("yyyyMMdd-hhmm")}/{request.EventGridEvent.Id}.json";
        if (await queueServiceClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken) != null)
        {
            _logger.LogInformation("{QueueName} created", request.Subscription.Destination.Properties.QueueName);
        }
        var response = await queueServiceClient.SendMessageAsync(jsonData, cancellationToken);
        
    }
}