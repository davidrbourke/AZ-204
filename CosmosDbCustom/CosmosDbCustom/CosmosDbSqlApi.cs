using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbCustom
{
    public class CosmosDbSqlApi
    {
        public static async Task Run()
        {
            var cosmosConnStr = ConfigurationManager.AppSettings.Get("cosmosConnStr");
            using var client = new CosmosClient(cosmosConnStr);

            var dbResponse = await client.CreateDatabaseIfNotExistsAsync("personDb");

            var db = dbResponse.Database;

            var containerProperties = new ContainerProperties
            {
                Id = "people",
                PartitionKeyPath = "/location"
            };

            var containerResponse = await db.CreateContainerIfNotExistsAsync(containerProperties);
            var container = containerResponse.Container;

            // await RunStoredProcedureExample(container);

            var id = Guid.NewGuid();

            var person = new Person
            {
                Id = id,
                Name = "David",
                Location = "UK"
            };

            var pKey = new PartitionKey(person.Location);
            var createItemResponse = await containerResponse.Container.CreateItemAsync<Person>(person, pKey,
                new ItemRequestOptions
                {
                    PreTriggers = new List<string>
                    {
                        "triggerAddType"
                    }
                });


            //var readPerson = await container.ReadItemAsync<Person>(id.ToString(), pKey);


            //var resource = readPerson.Resource;

            //resource.Name = "Bourke";

            //var updated = await container.ReplaceItemAsync(resource, resource.Id.ToString(), pKey);

            //var deletedResponse = await container.DeleteItemAsync<Person>(resource.Id.ToString(), pKey);

        }

        public static async Task RunStoredProcedureExample(Container container)
        {
            dynamic[] items = new dynamic[]
            {
                new
                {
                    id = Guid.NewGuid(),
                    location = "UK",
                    name = "Schumacher"
                }
            };

            var procs = container.Scripts;

            var result = await procs.ExecuteStoredProcedureAsync<string>
                ("newItem2", new PartitionKey("UK"), items);

            Console.WriteLine(result.Resource);

        }
    }
}
