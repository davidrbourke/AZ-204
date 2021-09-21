using System;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graph.Auth;

namespace GraphDemoConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientId = "";
            var scopes = new[] { "User.Read" };
            
            IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
                        .Create(clientId)
                        .Build();


            DeviceCodeProvider authProvider = new DeviceCodeProvider(publicClientApplication, scopes);
            var graphClient = new GraphServiceClient(authProvider);

            var me = await graphClient.Me.Request().GetAsync();

           
        }
    }
}
