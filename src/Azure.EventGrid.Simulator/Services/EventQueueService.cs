using System.Collections.Concurrent;
using Azure.EventGrid.Simulator.Commands;

namespace Azure.EventGrid.Simulator.Services
{
    public class EventQueueService: IEventQueueService
    {
        private ConcurrentQueue<InvokeWebHookCommand> _concurrentQueue = new();

        public void Enqueue(InvokeWebHookCommand command)
        {
            _concurrentQueue.Enqueue(command);
        }

        public InvokeWebHookCommand? Dequeue()
        {
            return _concurrentQueue.TryDequeue(out var command) ? command : null;
        }
    }


    public interface IEventQueueService
    {
        void Enqueue(InvokeWebHookCommand command);
        InvokeWebHookCommand? Dequeue();
    }
}
