using Newtonsoft.Json;

namespace Azure.EventGrid.Simulator.Settings
{
    public class SimulatorSettings
    {
        [JsonProperty(PropertyName = "topics", Required = Required.Always)]
        public TopicSettings[] Topics { get; set; } = Array.Empty<TopicSettings>();

    }
}
