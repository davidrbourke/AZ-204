// See https://aka.ms/new-console-template for more information
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using System.Text;

var client = new ClientManager();

//client.SendMessagesToEventHub().Wait();
//client.RetrieveMessagesFromEventHub().Wait();
client.ConsumerWithCheckpoint().Wait();

public class ClientManager
{
    private readonly string _eventHubConnStr = "";
    private readonly string _eventHubName = "";
    private readonly string _storageAccountConnStr = "";
    private readonly string _containerName = "";

    public async Task SendMessagesToEventHub()
    {
        Console.WriteLine("Start writing events");

        var producer = new EventHubProducerClient(_eventHubConnStr, _eventHubName);

        try
        {
            // Using a event data batch object validates message size <= 1MB
            using EventDataBatch eventBatch = await producer.CreateBatchAsync();

            for (var i = 0; i < 10; i++)
            {
                var eventData = new EventData($"Event #{i} {DateTime.UtcNow}");

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

        var producer = new EventHubProducerClient(_eventHubConnStr, _eventHubName);

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
            _eventHubConnStr,
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

    public async Task ConsumerWithCheckpoint()
    {
        var consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
        var storageClient = new BlobContainerClient(_storageAccountConnStr, _containerName);
        var processor = new EventProcessorClient(storageClient, consumerGroup, _eventHubConnStr, _eventHubName);

        processor.ProcessEventAsync += Processor_ProcessEventAsync;
        processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
        
        await processor.StartProcessingAsync();

        await Task.Delay(TimeSpan.FromSeconds(30));

        await processor.StopProcessingAsync();
    }

    private async Task Processor_ProcessEventAsync(Azure.Messaging.EventHubs.Processor.ProcessEventArgs arg)
    {
        Console.WriteLine("\tReceived event: {0}", Encoding.UTF8.GetString(arg.Data.Body.ToArray()));

        // Update the checkpoint in Azure Storage to only read new events on next run
        await arg.UpdateCheckpointAsync(arg.CancellationToken);
    }

    private Task Processor_ProcessErrorAsync(Azure.Messaging.EventHubs.Processor.ProcessErrorEventArgs arg)
    {
        Console.WriteLine($"\tPartition '{ arg.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
        Console.WriteLine(arg.Exception.Message);
        return Task.CompletedTask;
    }

}
