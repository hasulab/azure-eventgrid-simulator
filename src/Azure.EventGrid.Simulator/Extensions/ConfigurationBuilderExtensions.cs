namespace Azure.EventGrid.Simulator.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddCustomSimulatorConfigFileIfSpecified(this IConfigurationBuilder builder,
            IConfiguration configuration)
        {
            var configFileOverridden = configuration["ConfigFile"];

            if (!string.IsNullOrWhiteSpace(configFileOverridden))
            {
                if (!File.Exists(configFileOverridden))
                {
                    throw new FileNotFoundException("The specified ConfigFile could not be found.", configFileOverridden);
                }

                builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), configFileOverridden), false, false);
            }

            return builder;
        }

        public static IConfigurationBuilder AddEnvironmentVariablesAndCommandLine(this IConfigurationBuilder builder, string[] args)
        {
            return builder
                .AddEnvironmentVariables("ASPNETCORE_")
                .AddEnvironmentVariables("AEGS_")
                .AddCommandLine(args);
        }
        public static string EnvironmentName(this IConfiguration configuration)
        {
            return configuration["ENVIRONMENT"].Otherwise("Production");
        }
    }
}
