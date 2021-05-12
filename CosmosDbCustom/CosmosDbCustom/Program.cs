using Microsoft.Azure.Cosmos;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace CosmosDbCustom
{
    class Program
    {
        static async Task Main(string[] args)
        {

            await CosmosDbSqlApi.Run();

            // await CosmosDbTableApi.Run();
        }

    }
}
