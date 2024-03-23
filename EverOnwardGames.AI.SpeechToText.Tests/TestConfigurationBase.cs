using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverOnwardGames.AI.SpeechToText.Tests
{
    public class TestConfigurationBase
    {
        protected static IConfiguration _configuration { get; set; } = BuildConfiguration();


        protected static IConfiguration BuildConfiguration()
        {
            var appId = Environment.GetEnvironmentVariable("EOG_KEYVAULT_TEST_APPID", EnvironmentVariableTarget.User);
            var name = Environment.GetEnvironmentVariable("EOG_KEYVAULT_TEST_NAME", EnvironmentVariableTarget.User);
            var password = Environment.GetEnvironmentVariable("EOG_KEYVAULT_TEST_PASSWORD", EnvironmentVariableTarget.User);
            var tenant = Environment.GetEnvironmentVariable("EOG_KEYVAULT_TEST_TENANT", EnvironmentVariableTarget.User);
            var uri = Environment.GetEnvironmentVariable("EOG_KEYVAULT_TEST_URI", EnvironmentVariableTarget.User);

            var clientSecretCredential = new ClientSecretCredential(tenant, appId, password);

            return new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddAzureKeyVault(new Uri(uri), clientSecretCredential)
                .Build();
        }
    }
}
