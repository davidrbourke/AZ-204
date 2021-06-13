using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace StorageAccountQueueApp
{
    public class Program
    {
        private static string _connStr = "";
        public static async Task Main(string[] args)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_connStr);

            CloudQueueClient client = account.CreateCloudQueueClient();

            CloudQueue cloudQueue = client.GetQueueReference("demosample");

            CloudQueueMessage message = new CloudQueueMessage($"this is message on the queue {DateTime.Now}");

            await cloudQueue.AddMessageAsync(message);

            Console.WriteLine("Message added to queue");


            // Read from Queue
            await cloudQueue.FetchAttributesAsync();
            if (cloudQueue.ApproximateMessageCount > 0)
            {
                CloudQueueMessage dequeueMessage = await cloudQueue.GetMessageAsync();

                Console.WriteLine("Message downloaded - reads:)");
                Console.WriteLine(dequeueMessage.AsString);

                await cloudQueue.DeleteMessageAsync(dequeueMessage);
            }
        }
    }
}
