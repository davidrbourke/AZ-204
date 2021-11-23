// See https://aka.ms/new-console-template for more information
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;


var client = new ClientManager();

//client.SendMessagesToEventHub().Wait();
client.RetrieveMessagesFromEventHub().Wait();

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
            // Using a event data batch object validates message size <= 1MB
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

    public async Task SendMessagesToEventHubExplicitPartitionKey()
    {
        Console.WriteLine("Start writing events");

        var producer = new EventHubProducerClient(_connectionString, _eventHubName);

        try
        {
            // Using a event list like this doesn't not validate message size <= 1MB
            var events = new List<EventData>();
            
            for (var i = 0; i < 10; i++)
            {
                var eventData = new EventData($"Event #{i}");

                events.Add(eventData);
            }
            var options = new SendEventOptions();

            /************************************************************/
            // Events with same partition key are writen to same partition
            options.PartitionKey = "p1";
            /************************************************************/

            Console.WriteLine("Sending events to event hub");
            await producer.SendAsync(events, options);
        }
        finally
        {
            await producer.CloseAsync();
        }

        Console.WriteLine("Completed writing events");
    }

    public async Task RetrieveMessagesFromEventHub()
    {
        var consumer = new EventHubConsumerClient(
            EventHubConsumerClient.DefaultConsumerGroupName,
            _connectionString,
            _eventHubName);

        try
        {


            using var cancellationSource = new CancellationTokenSource();
            cancellationSource.CancelAfter(TimeSpan.FromSeconds(90));

            var maximumEvents = 100;
            var eventDataRead = new List<string>();

            await foreach (PartitionEvent partitionEvent in consumer.ReadEventsAsync(cancellationSource.Token))
            {

                var eventMessage = partitionEvent.Data.EventBody.ToString();
                eventDataRead.Add(eventMessage);

                Console.WriteLine(eventMessage);

                if (eventDataRead.Count > maximumEvents)
                {
                    break;
                }
            }
        }
        catch(Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
        }
        finally
        {
            await consumer.CloseAsync();
        }
    }
}
