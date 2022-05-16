using Refit;

namespace VaultService.Refit
{
    [Headers("Content-Type: application/json")]
    public interface IVaultClient
    {
        [Get("/v1/secret/data/{secretName}")]
        [Headers("X-Vault-Token:someRoot")]
        Task<string> GetSecret(string secretName);

        [Post("/v1/secret/data/{secretName}")]
        [Headers("X-Vault-Token:someRoot")]
        Task<string> AddSecret(string secretName, string key, string value);
    }
}
