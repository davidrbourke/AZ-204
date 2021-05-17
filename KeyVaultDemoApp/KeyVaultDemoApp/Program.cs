using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Text;

namespace KeyVaultDemoApp
{
    /// <summary>
    /// Demos reading from KeyVault using Service Principal
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Read from keyvault");

            // Setup in Azure:
            // -> Create a Service Principal in Azure AD using Azure CLI
            // az ad sp create-for-rbac -n {keyvaultname} --skip-assignemtn
            // The above generates a result that should be set an environment variables
            // appId = AZURE_CLIENT_ID
            // password = AZURE_CLIENT_SECRET
            // tenant = AZURE_TENANT_ID

            // Set access policy on KeyVault
            // az keyvault set-policy --name {keyvaultname} --spn "{appId from above}" --secret-permissions get list delete (etc, etc)
            var keyVaultUrl = "https://trainingvaultdrb.vault.azure.net/";

            // DefaultAzureCredentials will get the settings from above from the environment variables
            var secretClient = new SecretClient(vaultUri: new Uri(keyVaultUrl), credential: new DefaultAzureCredential());

            // Get a Secret
            KeyVaultSecret secret = secretClient.GetSecret("apppwd");

            Console.WriteLine($"{secret.Name}, {secret.Value}");

            // Get a Key
            var keyClient = new KeyClient(vaultUri: new Uri(keyVaultUrl), credential: new DefaultAzureCredential());

            KeyVaultKey key = keyClient.GetKey("drbenckey");

            // Encrypts using the key - these are Azure libraries
            var cyptoClient = new CryptographyClient(key.Id, credential: new DefaultAzureCredential());
            byte[] plaintext = Encoding.UTF8.GetBytes("text to encrypt");
            EncryptResult result = cyptoClient.Encrypt(EncryptionAlgorithm.RsaOaep256, plaintext);

            Console.WriteLine(Encoding.UTF8.GetString(result.Ciphertext));
        }
    }
}
