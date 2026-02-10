namespace MON.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, CancellationToken token = default);
        Task<T> GetAsync<T>(string key, CancellationToken token = default) where T : class;
        Task<object> GetFullAsync(string key);
        Task RefreshAsync(string key, CancellationToken token = default);
        Task RemoveAsync(string key, CancellationToken token = default);
        Task<List<string>> GetKeys(string pattern = "");
        Task ClearCache();

        Task<object> GetCacheServerInfo();
    }
}
