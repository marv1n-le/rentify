using System.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Rentify.Services.ExternalService.Redis;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _redisDb;

    public CacheService(IDistributedCache cache, IConnectionMultiplexer redisDb)
    {
        _cache = cache;
        _db = redisDb.GetDatabase();
        _redisDb = redisDb;
    }

    public async Task<int> GetVersionAsync(string resource)
    {
        string key = $"{resource}:version";
        int? version = await GetAsync<int?>(key);
        return version ?? 1;
    }

    public async Task IncreaseVersionAsync(string resource, TimeSpan? expiry = null)
    {
        string key = $"{resource}:version";
        int version = await GetAsync<int?>(key) ?? 1;
        version++;
        await SetAsync(key, version, expiry ?? TimeSpan.FromDays(7));
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var sw = Stopwatch.StartNew();
        var data = await _db.StringGetAsync(key);
        sw.Stop();
        Console.WriteLine($"[REDIS DIRECT GET] Key: {key}, Time: {sw.ElapsedMilliseconds}ms");

        if (data.IsNullOrEmpty)
            return default;

        return JsonConvert.DeserializeObject<T>(data!);
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var sw = Stopwatch.StartNew();
        var json = JsonConvert.SerializeObject(value);
        await _db.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(10));
        sw.Stop();
        Console.WriteLine($"[REDIS DIRECT SET] Key: {key}, Time: {sw.ElapsedMilliseconds}ms");
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}