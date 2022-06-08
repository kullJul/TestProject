using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Refit;
using StackExchange.Redis;
using WebApi.Refit;
using zipkin4net;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes ="IdentityApiKey")]
    public class RedisController : MyController
    {
        readonly IMockServiceClient mockServiceClient;

        public RedisController(IConfiguration configuration) : base(configuration)
        {
            mockServiceClient = RestService.For<IMockServiceClient>("http://mockservice:80");
        }

        [HttpGet("info")]
        public string GetInfo()
        {
            TraceLog("GET","Get info");
            return mockServiceClient.GetInfo().GetAwaiter().GetResult();
        }

        [HttpGet]
        public string GetRedisValue(string @key)
        {
            if (string.IsNullOrEmpty(key))
            {
                TraceLog("GET", "key is null or empty");
                return "key is null or empty";
            }
            TraceLog("GET", $"Get redis value for key = {key}");
            return mockServiceClient.GetRedisValue(key).GetAwaiter().GetResult();
        }

        [HttpPost]
        public string SetRedisValue(string @key, string @value)
        {
            if (string.IsNullOrEmpty(key))
            {
                TraceLog("POST", "key is null or empty");
                return "key is null or empty";
            }
            if (string.IsNullOrEmpty(value))
            {
                TraceLog("POST", "value is null or empty");
                return "value is null or empty";
            }
            TraceLog("POST", $"Set redis value = {value} for key = {key}");
            return mockServiceClient.SetRedisValue(key, value).GetAwaiter().GetResult();
        }
    }
}
