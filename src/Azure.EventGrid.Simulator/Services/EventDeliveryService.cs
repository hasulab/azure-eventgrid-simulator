using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Azure.EventGrid.Simulator.Commands;
using Azure.EventGrid.Simulator.Exceptions;
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
   //private List<Task> _runningTasks = new ();
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

        var _runningTasks = new ReadOnlyCollection<Task>(new List<Task>());

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug($"EventDeliveryService task doing background work.");

            var newTaskList = new List<Task>(_runningTasks);
            for (var i = _runningTasks.Count; i < _settings.ConcurrentEventsProcessing; i++)
            {
                var command = _queueService.Dequeue();
                if (command != null)
                {
                    var currentTask= Send(command, stoppingToken);
                    newTaskList.Add(currentTask);
                }
                else
                {
                    break;
                }
            }

            try
            {
                Task.WaitAny(newTaskList.ToArray(), stoppingToken);
                _runningTasks = new ReadOnlyCollection<Task>(newTaskList.Where(x=>x.IsCompleted == false).ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }

            await Task.Delay(_settings.CheckUpdateTime, stoppingToken);
        }

        _logger.LogDebug($"GracePeriod background task is stopping.");
    }
    private IEnumerable<InvokeCommandBase?> GetQueueMessages()
    {
        var command = _queueService.Dequeue();
        yield return command;
    }

    private async Task Send(InvokeCommandBase command, CancellationToken stoppingToken)
    {
        try
        {
            await _mediator.Send(command, stoppingToken);
        }
        catch (RetryException e)
        {
            Console.WriteLine(e);
        }
    }
}