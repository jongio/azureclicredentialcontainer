using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;

namespace AzureCliCredentialContainer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cred = new AzureCliCredential();
            var client = new KeyClient(new Uri("https://jongkv.vault.azure.net"), cred);
            var key = await client.GetKeyAsync("key1");

            Console.WriteLine(key.Value.Name);
        }
    }
}
