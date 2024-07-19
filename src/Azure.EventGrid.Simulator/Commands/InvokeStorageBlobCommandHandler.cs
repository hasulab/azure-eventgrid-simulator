using Azure.Storage.Blobs;
using MediatR;
using System.Text;
using Newtonsoft.Json;

namespace Azure.EventGrid.Simulator.Commands;

public class InvokeStorageBlobCommandHandler : IRequestHandler<InvokeStorageBlobCommand>
{
    private readonly ILogger<InvokeStorageBlobCommandHandler> _logger;

    public InvokeStorageBlobCommandHandler(ILogger<InvokeStorageBlobCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(InvokeStorageBlobCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending to Blob {BlobContainerName}", request.Subscription.Destination.Properties.QueueName);

        var blobServiceClient = new BlobServiceClient(request.Subscription.Destination.Properties.StorageConnectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(request.Subscription.Destination.Properties.BlobContainerName);
        var jsonData = JsonConvert.SerializeObject(request.EventGridEvent);
        var fileName = $"{DateTime.UtcNow.ToString("yyyyMMdd-hhmm")}/{request.EventGridEvent.Id}.json";

        var ingressBytes = Encoding.UTF8.GetBytes(jsonData);
        var contentStream = new MemoryStream(ingressBytes);

        await blobContainerClient.GetBlobClient(fileName).UploadAsync(contentStream, overwrite: true);

    }
}