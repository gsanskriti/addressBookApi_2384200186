using System;
using System.Threading.Tasks;

namespace AddressBookAPI.RepositoryLayer.Interface
{
    public interface IRedisCacheService
    {
        Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration);
        Task<T?> GetCacheValueAsync<T>(string key);
        Task<bool> RemoveCacheValueAsync(string key);
    }
}
