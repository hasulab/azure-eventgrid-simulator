using Azure.EventGrid.Simulator.Services;
using Azure.EventGrid.Simulator.Settings;
using MediatR;
using System.Reflection;

namespace Azure.EventGrid.Simulator.Extensions
{
    public static class ServiceCollectionExtensions
    {   public static IServiceCollection AddSimulatorServices(this IServiceCollection services, IConfiguration configuration)
		{
            var settings = new SimulatorSettings();
            configuration.Bind(settings);

            services.AddSingleton(_ => settings);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddHttpClient();
			services.Configure<EventDeliverySettings>(opt
				=> configuration.GetSection("EventDeliverySettings")
					.Bind(opt));
            services.AddHostedService<EventDeliveryService>();
            services.AddSingleton<IEventQueueService, EventQueueService>();

            return services;
		}
    }
}
