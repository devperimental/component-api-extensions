using Microsoft.Extensions.DependencyInjection;
using PlatformX.Common.Types.DataContract;

namespace PlatformX.Startup.Extensions
{
    public static class ConfigurationExtensions
    {
        public static BootstrapConfiguration AddBootstrapConfiguration(this IServiceCollection services)
        {
            var bootstrapConfiguration = new BootstrapConfiguration
            {
                EnvironmentName = Environment.GetEnvironmentVariable("EnvironmentName"),
                AspNetCoreEnvironmentName = Environment.GetEnvironmentVariable("AspNetCoreEnvironmentName"),
                Prefix = Environment.GetEnvironmentVariable("Prefix"),
                RoleKey = Environment.GetEnvironmentVariable("RoleKey"),
                PortalName = Environment.GetEnvironmentVariable("PortalName"),
                ServiceName = Environment.GetEnvironmentVariable("ServiceName"),
                Layer = Environment.GetEnvironmentVariable("Layer"),
                AzureRegion = Environment.GetEnvironmentVariable("AzureRegion"),
                AzureLocation = Environment.GetEnvironmentVariable("AzureLocation"),
                AzureTenantId = Environment.GetEnvironmentVariable("AzureTenantId"),
                AwsRegion = Environment.GetEnvironmentVariable("AwsRegion"),
                SsmPrefx = Environment.GetEnvironmentVariable("SsmPrefx"),
                LogMessages = Environment.GetEnvironmentVariable("LogMessages") == "Y",
                LogWarnings = Environment.GetEnvironmentVariable("LogWarnings") == "Y",
                LogErrors = Environment.GetEnvironmentVariable("LogErrors") == "Y",
                DBName = Environment.GetEnvironmentVariable("DBName"),
                PlatformServiceMetaData = Environment.GetEnvironmentVariable("PlatformServiceMetaData"),
                ServiceKeys = Environment.GetEnvironmentVariable("ServiceKeys"),
                RuntimeConfiguration = Environment.GetEnvironmentVariable("RuntimeConfiguration"),
                ServiceSymmetricKeyName = Environment.GetEnvironmentVariable("ServiceSymmetricKeyName"),
                ServerName = Environment.GetEnvironmentVariable("ServerName"),
                BuildNumber = Environment.GetEnvironmentVariable("BuildNumber"),
                ContainerType = Environment.GetEnvironmentVariable("ContainerType"),
                CorsOrigins = Environment.GetEnvironmentVariable("CorsOrigins"),
            };

            services.AddSingleton(bootstrapConfiguration);

            return bootstrapConfiguration;
        }
    }
}
