using System;
using System.Threading.Tasks;

namespace StorageAccountApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connStr = "";
            var key = "";

            //var storageAccount = new StorageAccountDemo(connStr);
            //await storageAccount.DownloadAllBlobsInContainer();
            //await storageAccount.UploadToContainer();

            var sas = new GenerateSaSas(key);
            var uri = sas.GenerateSasUrl();
            Console.WriteLine(uri.ToString());

        }
    }
}
