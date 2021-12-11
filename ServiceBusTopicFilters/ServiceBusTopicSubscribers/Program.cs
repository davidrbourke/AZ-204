using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace ServiceBusTopicSubscribers
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot Configuration = builder.Build();

            var publisherConnectionString = Configuration.GetConnectionString("SubscriberConnectionString");

            var topicName = Configuration["TopicName"];

            Console.WriteLine(topicName);


            var serviceBusPubClient = new ServiceBusClient(publisherConnectionString);

            var serviceBusReceiverOptions = new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete };
            var subNameA = "sub-a";
            var subNameB = "sub-b";
            ServiceBusReceiver serviceBusReceiverSubA = serviceBusPubClient.CreateReceiver(topicName, subNameA, serviceBusReceiverOptions);
            ServiceBusReceiver serviceBusReceiverSubB = serviceBusPubClient.CreateReceiver(topicName, subNameB, serviceBusReceiverOptions);

            IList<Task> receivers = new List<Task>();
            receivers.Add(ReceivedMessagesAsync(serviceBusReceiverSubA, subNameA));
            receivers.Add(ReceivedMessagesAsync(serviceBusReceiverSubB, subNameB));

            await Task.WhenAll(receivers);

            await serviceBusReceiverSubA.CloseAsync();
            await serviceBusReceiverSubB.CloseAsync();

            Console.WriteLine("All messages received");
            Console.ReadLine();
        }

        public static async Task ReceivedMessagesAsync(ServiceBusReceiver serviceBusReceiver, string subscriber)
        {
            while (true)
            {
                var receivedMessage = await serviceBusReceiver.ReceiveMessagesAsync(1, TimeSpan.FromSeconds(1));

                if (receivedMessage == null || receivedMessage.Count == 0)
                    break;

                foreach (var message in receivedMessage)
                {
                    var body = message.Body.ToString();
                    var msgPropNumber = message.ApplicationProperties["MsgProp"];
                    Console.WriteLine($"{subscriber}\t{msgPropNumber}\t{body}");
                }
            }
        }
    }
}