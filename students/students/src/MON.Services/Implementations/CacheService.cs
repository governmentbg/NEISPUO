namespace MON.Services.Implementations
{
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileSystemGlobbing.Internal;
    using Microsoft.Extensions.Options;
    using MON.Models.Configuration;
    using MON.Services.Interfaces;
    using MON.Shared.Extensions;
    using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
    using StackExchange.Redis;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    public class CacheService : ICacheService
    {
        private readonly int _slidingExpInterval = 280;
        private readonly int _absoluteExpInterval = 300; // Време за рефреш на кеша в секунди
        private readonly IDistributedCache _cache;
        private readonly CachingConfig _cachingConfig;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        private const string AbsoluteExpirationKey = "absexp";
        private const string SlidingExpirationKey = "sldexp";
        private const string DataKey = "data";
        private const long NotPresent = -1;

        public CacheService(IDistributedCache cache,
            IOptions<CachingConfig> cachingConfig)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(IDistributedCache));
            _cachingConfig = cachingConfig.Value ?? throw new ArgumentNullException(nameof(CachingConfig));
        }

        private async Task FlushDBAsync()
        {
            IServer server = await GetConnectionServer();

            // completely wipe ALL keys from database 0
            await server.FlushDatabaseAsync();
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken token = default)
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(_slidingExpInterval),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_absoluteExpInterval),
            };

            byte[] byteArr = value?.ToByteArray() ?? new byte[] {};
            await _cache.SetAsync(key, byteArr, options, token);
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken token = default) where T : class
        {
            byte[] result = await _cache.GetAsync(key, token);

            return result.FromByteArray<T>();
        }

        public async Task<object> GetFullAsync(string key)
        {
            IDatabase cache = await GetConnectionDatabase();
            if (cache == null)
            {
                return null;
            }

            HashEntry[] results = await cache.HashGetAllAsync(key);
            MapMetadata(results, out DateTimeOffset? absExpr, out TimeSpan? sldExpr, out object data);
            return new { data, absExpr, sldExpr };
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            return _cache.RefreshAsync(key, token);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            return _cache.RemoveAsync(key, token);
        }

        public async Task ClearCache()
        {
            if (_cachingConfig.UseRedis)
            {
                await FlushDBAsync();
            }
            else
            {
                // Използва паметта
                List<string> cacheKeys = await GetKeys();
                if (cacheKeys != null)
                {
                    foreach (string key in cacheKeys.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        await RemoveAsync(key);
                    }
                }
            }
        }

        public async Task<object> GetCacheServerInfo()
        {
            if (_cachingConfig.UseRedis)
            {
                IServer server = await GetConnectionServer();
                return await server.InfoAsync();
            }

            return null;
        }

        public async Task<List<string>> GetKeys(string pattern = "")
        {
            List<string> keys = new List<string>();
            if (_cachingConfig.UseRedis)
            {
                IServer server = await GetConnectionServer();

                // show all keys in database 0 that include "foo" in their name
                //foreach (var key in server.Keys(pattern: "*foo*"))
                foreach (RedisKey key in server.Keys(pattern: pattern))
                {
                    keys.Add(key);
                }
            }
            else
            {
                if (_cache is MemoryDistributedCache distribitedMemCache)
                {
                    FieldInfo field = typeof(MemoryDistributedCache).GetField("_memCache", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null && field.GetValue(distribitedMemCache) is MemoryCache memoryCache)
                    {
                        PropertyInfo prop = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (prop != null && prop.GetValue(memoryCache) is ICollection collection)
                        {
                            foreach (var item in collection)
                            {
                                PropertyInfo methodInfo = item.GetType().GetProperty("Key");
                                if (methodInfo != null)
                                {
                                    object val = methodInfo.GetValue(item);
                                    if (val != null)
                                    {
                                        keys.Add(val.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return keys;
        }

        private async Task<IServer> GetConnectionServer()
        {
            ConfigurationOptions option = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                AllowAdmin = true,
                EndPoints = { _cachingConfig.RedisConnectionString }
            };

            if (!string.IsNullOrWhiteSpace(_cachingConfig.RedisPassword))
            {
                option.Password = _cachingConfig.RedisPassword;
            }

            ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);

            _connectionLock.Wait();
            try
            {
                ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(option);

                // get the target server
                var server = redis.GetServer(option.EndPoints.First());

                return server;
            }
            finally
            {
                _connectionLock.Release();
            }

        }

        private async Task<IDatabase> GetConnectionDatabase()
        {
            ConfigurationOptions option = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                AllowAdmin = true,
                EndPoints = { _cachingConfig.RedisConnectionString }
            };

            if (!string.IsNullOrWhiteSpace(_cachingConfig.RedisPassword))
            {
                option.Password = _cachingConfig.RedisPassword;
            }

            ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);

            _connectionLock.Wait();
            try
            {
                ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(option);

                // get the target server
                var database = redis.GetDatabase();

                return database;
            }
            finally
            {
                _connectionLock.Release();
            }

        }

        private void MapMetadata(HashEntry[] results, out DateTimeOffset? absoluteExpiration, out TimeSpan? slidingExpiration, out object data)
        {
            absoluteExpiration = null;
            slidingExpiration = null;
            data = null;
            if (results == null)
            {
                return; 
            }

            var absExp = results.FirstOrDefault(x => x.Name.HasValue && x.Name.ToString().Equals(AbsoluteExpirationKey, StringComparison.OrdinalIgnoreCase));
            if(absExp != null && absExp.Value != NotPresent && long.TryParse(absExp.Value, out long absoluteExpirationTicks))
            {
                absoluteExpiration = new DateTimeOffset(absoluteExpirationTicks, TimeSpan.Zero);
            }


            var sldExp = results.FirstOrDefault(x => x.Name.HasValue && x.Name.ToString().Equals(SlidingExpirationKey, StringComparison.OrdinalIgnoreCase));
            if (sldExp != null && sldExp.Value != NotPresent && long.TryParse(sldExp.Value, out long slidingExpirationTicks))
            {
                slidingExpiration = new TimeSpan(slidingExpirationTicks);
            }

            var dataVal = results.FirstOrDefault(x => x.Name.HasValue && x.Name.ToString().Equals(DataKey, StringComparison.OrdinalIgnoreCase));
            if (dataVal != null && sldExp.Value != NotPresent)
            {
                byte[] result = (byte[])dataVal.Value;

                data = result.FromByteArray<object>();
            }
        }
    }
}
