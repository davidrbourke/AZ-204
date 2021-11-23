// See https://aka.ms/new-console-template for more information
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;


var client = new ClientManager();

client.SendMessagesToEventHub().Wait();

public class ClientManager
{
    private readonly string _connectionString = "";
    private readonly string _eventHubName = "event-hub-1";

    public async Task SendMessagesToEventHub()
    {
        Console.WriteLine("Start writing events");

        var producer = new EventHubProducerClient(_connectionString, _eventHubName);

        try
        {
            using EventDataBatch eventBatch = await producer.CreateBatchAsync();

            for (var i = 0; i < 10; i++)
            {
                var eventData = new EventData($"Event #{i}");

                if (!eventBatch.TryAdd(eventData))
                {
                    throw new Exception($"The event at index {i} could not be added");
                }
            }

            Console.WriteLine("Sending events to event hub");
            await producer.SendAsync(eventBatch);
        }
        finally
        {
            await producer.CloseAsync();
        }

        Console.WriteLine("Completed writing events");
    }

    public void RetrieveMessagesFromEventHub()
    {

    }
}
