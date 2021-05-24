using StackExchange.Redis;
using System;
using System.Text.Json;

namespace RedisCacheDemo
{
    class Program
    {
        private static Lazy<ConnectionMultiplexer> _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string connection = "<insert key string from azure>";
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

            IDatabase cache = AppRedisConnection.GetDatabase();

            var customer = new Customer
            {
                Id = 1,
                Name = "David",
                City = "Chicago"
            };

            // Set in cache
            cache.StringSet($"Customer_Id{customer.Id}", JsonSerializer.Serialize(customer), TimeSpan.FromMinutes(60));

            // Get from cache
            Customer loadedCustomer = JsonSerializer.Deserialize<Customer>(cache.StringGet($"Customer_Id{customer.Id}"));

            Console.WriteLine(loadedCustomer.Id);
            Console.WriteLine(loadedCustomer.Name);
            Console.WriteLine(loadedCustomer.City);
        }
    }
}
