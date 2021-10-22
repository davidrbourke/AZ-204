using ServiceStack.Redis;
using StackExchange.Redis;
using System;
using System.Text.Json;

namespace RedisCacheDemo
{
    class Program
    {
        private static string connectionString = "<connstringhere>";

        private static Lazy<ConnectionMultiplexer> _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string connection = connectionString;
            return ConnectionMultiplexer.Connect(connection);
        });

        public static ConnectionMultiplexer AppRedisConnection
        {
            get
            {
                return _lazyConnection.Value;
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Azure redis cache demo");

            CommandsInATransaction();

            IDatabase cache = AppRedisConnection.GetDatabase();
            
            //var customer = new Customer
            //{
            //    Id = 1,
            //    Name = "David",
            //    City = "Chicago"
            //};

            //// Set in cache
            //cache.StringSet($"Customer_Id{customer.Id}", JsonSerializer.Serialize(customer), TimeSpan.FromMinutes(60));

            //// Get from cache
            //Customer loadedCustomer = JsonSerializer.Deserialize<Customer>(cache.StringGet($"Customer_Id{customer.Id}"));

            //Console.WriteLine(loadedCustomer.Id);
            //Console.WriteLine(loadedCustomer.Name);
            //Console.WriteLine(loadedCustomer.City);
        }

        private static void CommandsInATransaction()
        {
            var customerA = new Customer
            {
                Id = 2,
                Name = "Smith",
                City = "Sydney"
            };
            var customerB = new Customer
            {
                Id = 3,
                Name = "O'Neil",
                City = "Belfast"
            };

            using var redisclient = new RedisClient(connectionString);
            using var transaction = redisclient.CreateTransaction();

            transaction.QueueCommand(c => c.Set($"Customer_Id{customerA.Id}", JsonSerializer.Serialize(customerA), TimeSpan.FromMinutes(30)));
            transaction.QueueCommand(c => c.Set($"Customer_Id{customerB.Id}", JsonSerializer.Serialize(customerB), TimeSpan.FromMinutes(30)));

            var transactionResult = transaction.Commit();

            Console.WriteLine($"Written to cache? {transactionResult}");
        }
    }
}
