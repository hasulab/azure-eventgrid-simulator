using System.Text;
using Newtonsoft.Json;

namespace Azure.EventGrid.Simulator.Settings;

public class SubscriptionSettings
{
    [JsonProperty(PropertyName = "name", Required = Required.Always)]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "destination", Required = Required.Always)]
    public SubscriptionDestination Destination { get; set; }

    [JsonProperty(PropertyName = "filter", Required = Required.Default)]
    public FilterSetting Filter { get; set; }

    [JsonIgnore]
    public SubscriptionValidationStatus ValidationStatus { get; set; }

}

public class SubscriptionDestination
{
    [JsonProperty(PropertyName = "endpointType", Required = Required.Always)]
    public string EndpointType { get; set; }

    [JsonProperty(PropertyName = "properties", Required = Required.Always)]
    public DestinationProperties Properties { get; set; }

}

public class DestinationProperties
{
    private readonly DateTime _expired = DateTime.UtcNow.AddMinutes(5);

    [JsonProperty(PropertyName = "endpoint", Required = Required.Always)]
    public string Endpoint { get; set; }

    [JsonProperty(PropertyName = "blobContainerName", Required = Required.AllowNull)]
    public string BlobContainerName { get; set; }

    [JsonProperty(PropertyName = "disableValidation", Required = Required.Default)]
    public bool DisableValidation { get; set; }

    [JsonProperty(PropertyName = "disabled", Required = Required.Default)]
    public bool Disabled { get; set; }

    [JsonIgnore]
    public Guid ValidationCode => new(Encoding.UTF8.GetBytes(Endpoint).Reverse().Take(16).ToArray());

    [JsonIgnore]
    public bool ValidationPeriodExpired => DateTime.UtcNow > _expired;
}