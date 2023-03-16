using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;
using Microsoft.Extensions.DependencyInjection;
using PlatformX.Secrets.Aws;
using PlatformX.Secrets.Shared.Behaviours;

namespace PlatformX.Startup.Extensions
{
    public static class AwsExtensions
    {
        public static void AddSecretsManager(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IAmazonSecretsManager), c => {
                var region = RegionEndpoint.GetBySystemName(RegionEndpoint.APSoutheast2.SystemName);
                var config = new AmazonSecretsManagerConfig { RegionEndpoint = region };

                return new AmazonSecretsManagerClient(config);
            });

            services.AddSingleton(typeof(ISecretsManagerCache), c =>
            {
                var amazonSecretsManager = c.GetRequiredService<IAmazonSecretsManager>();
                uint CACHE_TIME = 300000;
                var secretCacheConfiguration = new SecretCacheConfiguration { CacheItemTTL = CACHE_TIME };
                return new SecretsManagerCache(amazonSecretsManager, secretCacheConfiguration);
            });

            services.AddSingleton<ISecretClient, AwsSecretsManagerClient>();
        }
    }
}
