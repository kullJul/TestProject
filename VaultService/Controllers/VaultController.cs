using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Refit;
using VaultService.Refit;

namespace VaultService.Controllers
{
    [Route("api/[controller]")]
    public class VaultController : ControllerBase
    {
        readonly IVaultClient vaultClient;

        public VaultController()
        {
            vaultClient = RestService.For<IVaultClient>("http://vault:8200");
        }

        [HttpGet("info")]
        public string GetInfo()
        {
            return "Vault controller";
        }

        [HttpGet]
        public string GetSecret(string @secretName)
        {
            var result = vaultClient.GetSecret(secretName).GetAwaiter().GetResult();
            var res = JObject.Parse(result).GetValue("data").First();
           
            return res.ToString();
        }

        [HttpPost]
        public string AddSecret(string @secretName, string @Key, string @value)
        {
            return vaultClient.AddSecret(secretName, Key, value).GetAwaiter().GetResult();
        }
    }
}
