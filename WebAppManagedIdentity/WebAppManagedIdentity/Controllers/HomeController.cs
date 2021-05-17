using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using WebAppManagedIdentity.Models;

namespace WebAppManagedIdentity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private string _secretIdentifier = "https://trainingvaultdrb.vault.azure.net/secrets/apppwd/4601676f4944499a9d21914f792ef4bd";

        public async Task<IActionResult> Index()
        {
            // Create Token Provider - will use Identity on Resource to generate a token
            // Setup:
            // In Azure portal
            // -> App Service
            // -> Identity
            // -> set System Assigned Identity: On
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            
            // Connect to keyvault with Managed Identity generated token
            // Setup:
            // In Azure portal
            // -> Key Vault
            // -> Access Policies
            // -> Add Access Policy
            // -> Select Principal (search for the App Identity)
            // -> Add Permissions and Save
            var secretClient = new KeyVaultClient
                (new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            // Access Secret
            var secret = await secretClient.GetSecretAsync(_secretIdentifier)
                .ConfigureAwait(false);

            ViewBag.Sec = secret.Value;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
