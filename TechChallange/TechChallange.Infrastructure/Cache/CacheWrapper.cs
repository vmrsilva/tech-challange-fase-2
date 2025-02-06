using Microsoft.Extensions.Caching.Distributed;
using TechChallange.Domain.Cache;

namespace TechChallange.Infrastructure.Cache
{
    public class CacheWrapper : ICacheWrapper
    {
        private readonly IDistributedCache _distributedCache;

        public CacheWrapper(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _distributedCache.GetStringAsync(key, cancellationToken);
        }

        public async Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            await _distributedCache.SetStringAsync(key, value, options, cancellationToken);
        }
    }
}
