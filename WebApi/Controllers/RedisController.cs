using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class RedisController : ControllerBase
    {
        readonly IDatabase db;
        public RedisController(IDatabase db)
        {
            this.db = db;
        }

        [HttpGet("info")]
        public string GetInfo()
        {
            return "Redis controller";
        }

        [HttpGet]
        public string GetRedisValue(string @key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return "key is null or empty";
            }
            return db.StringGet(key);
        }

        [HttpPost]
        public string SetRedisValue(string @key, string @value)
        {
            db.StringSet(key, value);
            return "Done";
        }
    }
}
