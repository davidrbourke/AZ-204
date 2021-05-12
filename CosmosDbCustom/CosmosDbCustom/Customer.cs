using Microsoft.Azure.Cosmos.Table;

namespace CosmosDbCustom
{
    public class Customer : TableEntity
    {
        //public string Id { get; set; }
        //public string City { get; set; }
        public string Name { get; set; }

        public Customer()
        {
        }

        public Customer(string city, string id, string name)
        {
            //City = city;
            //Id = id;
            Name = name;

            PartitionKey = city;
            RowKey = id;
        }
    }
}
