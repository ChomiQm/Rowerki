using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;

//https://learn.microsoft.com/en-us/azure/key-vault/secrets/quick-create-net?tabs=azure-cli
namespace Project1.Configurations
{
    public class KeyVaultImplementation
    {

        public static string GetSecretFromKeyVault(string secretName)
        {
            string keyVaultUri = Environment.GetEnvironmentVariable("VaultUri") ?? "";
            if (keyVaultUri == null)
            {
                throw new ArgumentNullException();
            }

            try
            {
                var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
                var secret = client.GetSecret(secretName);

                return secret.Value.Value;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught: {0}", e.Message);
                return "";
            }

        }

    }
}

