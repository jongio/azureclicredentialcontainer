using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using System.Linq;

namespace AzureCliCredentialContainer
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.GetFiles("/root/.azure").ToList().ForEach(f => Console.WriteLine(f));
            /*
                        try
                        {
                            var cred = new AzureCliCredential();
                            var client = new KeyClient(new Uri("https://jongkv.vault.azure.net"), cred);
                            var key = await client.GetKeyAsync("key1");
                            Console.WriteLine(key.Value.Name);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        */
            while (true) { }
        }
    }
}
