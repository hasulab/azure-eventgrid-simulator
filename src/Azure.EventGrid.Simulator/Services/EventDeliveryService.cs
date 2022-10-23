using System.Collections.Concurrent;
using Azure.EventGrid.Simulator.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Azure.EventGrid.Simulator.Services;

public class EventDeliveryService : BackgroundService
{
    private readonly ILogger<EventDeliveryService> _logger;
    private readonly IEventQueueService _queueService;
    private readonly IMediator _mediator;
    private readonly EventDeliverySettings _settings;
    private List<Task> _runningTasks = new ();
    private object syncObj = new object();
    public EventDeliveryService(ILogger<EventDeliveryService> logger,
        IOptions<EventDeliverySettings> settings,
        IEventQueueService queueService,
        IMediator mediator)
    {
        _logger = logger;
        _queueService = queueService;
        _mediator = mediator;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug($"EventDeliveryService is starting.");

        stoppingToken.Register(() =>
            _logger.LogDebug($" EventDeliveryService background task is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug($"EventDeliveryService task doing background work.");

            var command = _queueService.Dequeue();
            if (command != null)
            {
                await _mediator.Send(command, stoppingToken)
                    .ContinueWith(t =>
                    {
                        //lock (syncObj)
                        //{
                        //    _runningTasks.Remove(t);
                        //}
                    }, stoppingToken);
                
                //lock (syncObj)
                //{
                //    _runningTasks.Add(task);
                //}
            }

            await Task.Delay(_settings.CheckUpdateTime, stoppingToken);
        }

        _logger.LogDebug($"GracePeriod background task is stopping.");
    }
}