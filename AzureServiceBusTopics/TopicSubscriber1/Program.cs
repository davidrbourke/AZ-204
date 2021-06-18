using Azure.Messaging.ServiceBus;
using System;
using System.Text;
using System.Threading.Tasks;

namespace TopicSubscriber1
{
    class Program
    {
        private static ServiceBusClient _topicClient;
        private static ServiceBusProcessor _processor;
        private static string _subscriptionName = "demosubscription1";
        //private static string _subscriptionName = "demosubscription2"; 

        static async Task Main(string[] args)
        {
            var connStr = "";
            var topicName = "demotopic1";

            _topicClient = new ServiceBusClient(connStr);
            _processor = _topicClient.CreateProcessor(topicName, _subscriptionName, new ServiceBusProcessorOptions());

            try
            {
                _processor.ProcessMessageAsync += MessageHandler;
                _processor.ProcessErrorAsync += ErrorHandler;

                await _processor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();
                
                Console.WriteLine("Stopping");

                await _processor.StopProcessingAsync();

                Console.WriteLine("Stopped");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                await _processor.DisposeAsync();
                await _topicClient.DisposeAsync();
            }

        }

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body} from subscription: {_subscriptionName}");

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
