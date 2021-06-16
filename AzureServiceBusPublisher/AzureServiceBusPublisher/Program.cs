using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureServiceBusPublisher
{
    class Program
    {
        static IQueueClient queueClient;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Service Bus Publisher");

            if (args.Length < 1 || args[0] != "P" || args[0] != "S")
            {
                Console.WriteLine("Provide a parameter: P (Publisher), S (Subscriber");
                return;
            }

            Console.WriteLine("Service Bus Publisher");
            
            // A Shared Access Policy must be created on the Service Bus Namespace or on the Queue.
            // Once the connection string is available on the policy in Azure, copy it here and remove the entity part at the end.
            // Aslo set the queue name in queueEntityName.
            var connStr = "";
            var queueEntityName = "";
            queueClient = new QueueClient(connStr, queueEntityName);

            switch (args[0])
            {
                case "P":
                    await RunAsPublisher();
                    break;
                case "S":
                    RunAsSubscriber();
                    break;
                default:
                    break;
            }

            Console.ReadKey();
        }

        static async Task RunAsPublisher()
        {
            for (int i = 0; i < 100; i++)
            {
                // Messages can only be written as bytes.
                var message = new Message(Encoding.UTF8.GetBytes($"Message {i}"));
                await queueClient.SendAsync(message);
            }

            Console.ReadKey();
        }

        static void RunAsSubscriber()
        {
            // Configure message options
            // The exception handler delegate must be passed in to the constructor.
            var options = new MessageHandlerOptions(HandleException)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            // Register the message handler, passing it in as a delegate
            queueClient.RegisterMessageHandler(HandleMessage, options);

            Console.ReadKey();
        }

        // This is the actual message handler that will process each message
        static async Task HandleMessage(Message msg, CancellationToken token)
        {
            // Decode from bytes to string
            var msgTxt = Encoding.UTF8.GetString(msg.Body);
            Console.WriteLine($"Message received: {msgTxt}");

            // As the AutoComplete option is set to false, the message must be exlicitly set to completed.
            await queueClient.CompleteAsync(msg.SystemProperties.LockToken);
        }

        // The method for handling exceptions.
        static Task HandleException(ExceptionReceivedEventArgs args)
        {
            Console.WriteLine(args.Exception);
            return Task.CompletedTask;
        }
    }
}
