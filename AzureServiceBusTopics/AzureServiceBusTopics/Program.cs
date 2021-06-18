using Azure.Messaging.ServiceBus;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBusTopics
{
    class Program
    {
        private static ServiceBusClient _topicClient;
        private static ServiceBusSender _sender;

        static async Task Main(string[] args)
        {
            var connStr = "Endpoint=sb://davidrbourkesb.servicebus.windows.net/;SharedAccessKeyName=Az204;SharedAccessKey=NKExCrL36Qi0YZouSgEyYxtB9GNY1dtAK7oPEVomnsk=";
            var topicName = "demotopic1";
            //var subscriptName "demosubscription1";

            _topicClient = new ServiceBusClient(connStr);
            _sender = _topicClient.CreateSender(topicName);

            await PublishMessageToTopic();
        }

        private static async Task PublishMessageToTopic()
        {
            using ServiceBusMessageBatch messageBatch = await _sender.CreateMessageBatchAsync();

            for (int i = 0; i < 10; i++)
            {
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                {
                    throw new Exception($"The message {i} is too large to fit in the batch");
                }
            }

            try
            {
                await _sender.SendMessagesAsync(messageBatch);
                Console.WriteLine("A batch of message has been sent to the topic");
            }
            finally
            {
                await _sender.DisposeAsync();
                await _topicClient.DisposeAsync();
            }

            Console.WriteLine("Press enter to end the application");
            Console.ReadKey();
        }
    }
}
