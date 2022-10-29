using System.Collections.Concurrent;
using Azure.EventGrid.Simulator.Commands;

namespace Azure.EventGrid.Simulator.Services
{
    public class EventQueueService: IEventQueueService
    {
        private readonly ConcurrentQueue<InvokeCommandBase> _concurrentQueue = new();

        public void Enqueue(InvokeCommandBase command)
        {
            _concurrentQueue.Enqueue(command);
        }

        public InvokeCommandBase? Dequeue()
        {
            return _concurrentQueue.TryDequeue(out var command) ? command : null;
        }
    }


    public interface IEventQueueService
    {
        void Enqueue(InvokeCommandBase command);
        InvokeCommandBase? Dequeue();
    }
}
