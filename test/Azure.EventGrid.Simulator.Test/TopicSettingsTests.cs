using Azure.EventGrid.Simulator.Extensions;
using Azure.EventGrid.Simulator.Models;
using Azure.EventGrid.Simulator.Settings;
using Newtonsoft.Json;

namespace Azure.EventGrid.Simulator.Tests
{
    public class TopicSettingsTests
    {
        private TopicSettings _topicSettings = new TopicSettings();

        public TopicSettingsTests()
        {
            _topicSettings=new TopicSettings
            {
                Name = "testTopic",
                Port = 8080,
                Key = "testKey",
                Subscribers = new []
                {
                    new SubscriptionSettings
                    {
                        Name = "Test1",
                        DisableValidation = true,
                        Disabled = false,
                        ValidationStatus = SubscriptionValidationStatus.ValidationSuccessful,
                        Endpoint = "test1.Empty",
                        Filter = new FilterSetting
                        {
                            IncludedEventTypes = new List<string>()
                        }
                    },
                    new SubscriptionSettings
                    {
                        Name = "Test2",
                        DisableValidation = true,
                        Disabled = false,
                        ValidationStatus = SubscriptionValidationStatus.ValidationSuccessful,
                        Endpoint = "test2.FilterSetting",
                        Filter = new FilterSetting
                        {
                            IncludedEventTypes = new List<string>
                            {
                                Test2_FilterSetting
                            }
                        }
                    }
                }
            };
        }

        private const string Test2_FilterSetting = "Test2.FilterSetting";

        [Fact]
        public void Test1()
        {
            var @event = new EventGridEvent
            {
                EventType = Test2_FilterSetting,
                Data = new { Id = 1 }
            };
            
            var json= JsonConvert.SerializeObject(_topicSettings);

            var counter = 0;
            foreach (var subscription in _topicSettings.Subscribers)
            {
                if (subscription.Filter.AcceptsEvent(@event))
                {
                    counter++;
                }
            }
        }
    }
}