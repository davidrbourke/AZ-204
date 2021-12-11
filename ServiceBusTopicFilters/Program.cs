using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace ServiceBusTopicFilters
{
    public class Program
    {
        private static int messageCounter = 0;

        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot Configuration = builder.Build();

            var publisherConnectionString = Configuration.GetConnectionString("PublisherConnectionString");

            var topicName = Configuration["TopicName"];

            Console.WriteLine(topicName);


            var serviceBusPubClient = new ServiceBusClient(publisherConnectionString);
            ServiceBusSender serviceBusSender = serviceBusPubClient.CreateSender(topicName);

            while (messageCounter < 100)
            {
                await SendMessagesAsync(serviceBusSender);
            }
        }

        public static async Task SendMessagesAsync(ServiceBusSender serviceBusSender)
        {
            ServiceBusMessage serviceBusMessage = new();
            var messageBody = $"Message: {messageCounter++}";
            serviceBusMessage.Body = BinaryData.FromString(messageBody);
            serviceBusMessage.ApplicationProperties.Add("MsgProp", messageCounter % 2);
            await serviceBusSender.SendMessageAsync(serviceBusMessage);

            Console.WriteLine($"Messages sent:\tMsgProp {messageCounter}\tMessage: {messageBody}");
        }
    }
}