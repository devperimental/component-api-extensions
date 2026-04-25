using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Logging;
using PlatformX.Startup.Behaviours;
using System.Net;

namespace PlatformX.Startup
{
    public class AwsSecretsManagerClient : ISecretClient
    {
        private readonly ISecretsManagerCache _secretsManagerCache;

        private readonly IAmazonSecretsManager _amazonSecretsManager;

        private readonly ILogger<AwsSecretsManagerClient> _logger;

        public AwsSecretsManagerClient(ISecretsManagerCache secretsManagerCache, IAmazonSecretsManager amazonSecretsManager, ILogger<AwsSecretsManagerClient> logger)
        {
            _secretsManagerCache = secretsManagerCache;
            _amazonSecretsManager = amazonSecretsManager;
            _logger = logger;
        }

        public string GetSecret(string keyName)
        {
            return GetSecretAsync(keyName).Result;
        }

        public async Task<string> GetSecretAsync(string keyName)
        {
            return await _secretsManagerCache.GetSecretString(keyName);
        }

        public void PurgeDeletedSecret(string keyName)
        {
            throw new NotImplementedException();
        }

        public bool SetSecret(string keyName, string value)
        {
            return SetSecretAsync(keyName, value).Result;
        }

        public async Task<bool> SetSecretAsync(string keyName, string value)
        {
            CreateSecretResponse createSecretResponse = await _amazonSecretsManager.CreateSecretAsync(new CreateSecretRequest
            {
                Name = keyName,
                SecretString = value
            });

            return createSecretResponse.HttpStatusCode == HttpStatusCode.Created || createSecretResponse.HttpStatusCode == HttpStatusCode.OK;
        }

        public void StartDeleteSecret(string keyName)
        {
            throw new NotImplementedException();
        }
    }
}
