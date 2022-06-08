using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace MockService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : MyController
    {
        readonly IDatabase db;
        public RedisController(IDatabase db, IConfiguration configuration) :base(configuration)
        {
            this.db = db;
        }

        [HttpGet("info")]
        public string GetInfo()
        {
            TraceLog("GET", "Get info");
            return "Redis controller";
        }

        [HttpGet]
        public string GetRedisValue(string @key)
        {
            if (string.IsNullOrEmpty(key))
            {
                TraceLog("key is null or empty");
                return "key is null or empty";
            }
            TraceLog("GET", $"Get redis value for key = {key}");
            return db.StringGet(key);
        }

        [HttpPost]
        public string SetRedisValue(string @key, string @value)
        {
            db.StringSet(key, value);
            TraceLog("POST", $"Set redis value = {value} for key = {key}");
            return "Done";
        }
    }
}
