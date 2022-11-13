using System.Collections.Concurrent;
using Azure.EventGrid.Simulator.Commands;

namespace Azure.EventGrid.Simulator.Services
{
    public class EventQueueService: IEventQueueService
    {
        private readonly ILogger<EventQueueService> _logger;
        private readonly ConcurrentQueue<InvokeCommandBase> _concurrentQueue = new();
        ManualResetEventSlim _manualResetEvent = new ManualResetEventSlim(true);
        public EventQueueService(ILogger<EventQueueService> logger)
        {
            _logger = logger;
        }

        public void Enqueue(InvokeCommandBase command)
        {
            _logger.LogDebug($"Added {command.TopicName}: {command.EventGridEvent.EventType}");
            _concurrentQueue.Enqueue(command);

            if (!_manualResetEvent.IsSet)
            {
                _manualResetEvent.Set();
            }
        }

        public InvokeCommandBase? Dequeue()
        {
            _logger.LogDebug("waiting for message");

            _manualResetEvent.Wait();
            if (_concurrentQueue.TryDequeue(out var command))
            {
                _logger.LogDebug($"Found a {command.TopicName}: {command.EventGridEvent.EventType}");
                return command;
            }
            else
            {
                _logger.LogDebug("no message found");
                _manualResetEvent.Reset();
                return null;
            }
        }
    }


    public interface IEventQueueService
    {
        void Enqueue(InvokeCommandBase command);
        InvokeCommandBase? Dequeue();
    }
}
