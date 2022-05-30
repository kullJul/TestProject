using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Refit;
using StackExchange.Redis;
using WebApi.Refit;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes ="IdentityApiKey")]
    public class RedisController : ControllerBase
    {
        readonly IMockServiceClient mockServiceClient;
        public RedisController()
        {
            mockServiceClient = RestService.For<IMockServiceClient>("http://mockservice:80");
        }

        [HttpGet("info")]
        public string GetInfo()
        {
            return mockServiceClient.GetInfo().GetAwaiter().GetResult();
        }

        [HttpGet]
        public string GetRedisValue(string @key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return "key is null or empty";
            }
            return mockServiceClient.GetRedisValue(key).GetAwaiter().GetResult();
        }

        [HttpPost]
        public string SetRedisValue(string @key, string @value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return "key is null or empty";
            }
            if (string.IsNullOrEmpty(value))
            {
                return "value is null or empty";
            }
            return mockServiceClient.SetRedisValue(key, value).GetAwaiter().GetResult();
        }
    }
}
