namespace Azure.EventGrid.Simulator.Settings
{
    public class EventDeliverySettings
    {
        public int CheckUpdateTime { get; set; } = 100;
        public int ConcurrentEventsProcessing { get; set; } = 5;
    }
}
