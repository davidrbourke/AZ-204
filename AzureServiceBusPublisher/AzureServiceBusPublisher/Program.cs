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
                var message = new Message(Encoding.UTF8.GetBytes($"Message {i}"));
                await queueClient.SendAsync(message);
            }

            Console.ReadKey();
        }

        static void RunAsSubscriber()
        {
            var options = new MessageHandlerOptions(HandleException)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(HandleMessage, options);

            Console.ReadKey();
        }

        static async Task HandleMessage(Message msg, CancellationToken token)
        {
            var msgTxt = Encoding.UTF8.GetString(msg.Body);
            Console.WriteLine($"Message received: {msgTxt}");
            await queueClient.CompleteAsync(msg.SystemProperties.LockToken);
        }

        static Task HandleException(ExceptionReceivedEventArgs args)
        {
            Console.WriteLine(args.Exception);
            return Task.CompletedTask;
        }
    }
}
