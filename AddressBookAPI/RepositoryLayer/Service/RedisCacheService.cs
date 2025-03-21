using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using AddressBookAPI.RepositoryLayer.Interface;

namespace AddressBookAPI.RepositoryLayer.Service
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _database = _redis.GetDatabase();
        }

        // Store data in Redis (Generic)
        public async Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration)
        {
            var jsonData = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, jsonData, expiration);
        }

        // Retrieve data from Redis (Generic)
        public async Task<T?> GetCacheValueAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (!value.HasValue) return default;

            if (typeof(T) == typeof(string))
            {
                return (T)(object)value.ToString();  // Handle strings separately
            }

            return JsonSerializer.Deserialize<T>(value);
        }

        // Remove a key from Redis cache
        public async Task<bool> RemoveCacheValueAsync(string key)
        {
            return await _database.KeyDeleteAsync(key);
        }
    }
}
