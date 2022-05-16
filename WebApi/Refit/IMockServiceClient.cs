using Refit;

namespace WebApi.Refit
{
    [Headers("Content-Type: application/json")]
    public interface IMockServiceClient
    {
        [Get("/api/redis/info")]
        Task<string> GetInfo();
        [Get("/api/redis")]
        Task<string> GetRedisValue(string key);
        [Post("/api/redis")]
        Task<string> SetRedisValue(string key, string value);
    }
}
