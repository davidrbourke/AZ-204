using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbCustom
{
    public class CosmosDbTableApi
    {
        public static async Task Run()
        {
            var cosmosConnStr = ConfigurationManager.AppSettings.Get("cosmosTableConnStr");

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(cosmosConnStr);
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            
            CloudTable cloudTable = cloudTableClient.GetTableReference("peopleTable2");

            var isCreated = await cloudTable.CreateIfNotExistsAsync();

            var customer = new Customer("NY", "4", "David");

            // Insert
            TableOperation op = TableOperation.Insert(customer);
            TableResult res = await cloudTable.ExecuteAsync(op);

            customer.Name = "Bourke";

            // Update
            TableOperation updateOp = TableOperation.InsertOrReplace(customer);
            TableResult updateRes = await cloudTable.ExecuteAsync(updateOp);

            // Retrieve
            TableOperation retrieveOp = TableOperation.Retrieve<Customer>("NY", "4"); //, new List<string> { "Name" }); // Optional 3rd param
            TableResult retrieveRes = await cloudTable.ExecuteAsync(retrieveOp);

            var retCust = retrieveRes.Result as Customer;

            Console.WriteLine(retCust);

            // Delete
            TableOperation delOp = TableOperation.Delete(retCust);
            TableResult delRes = await cloudTable.ExecuteAsync(delOp);
        }
    }
}
