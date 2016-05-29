using System;
using System.Runtime.Caching;

namespace nerve.core.data.lightfoot
{
    public static class MemoryCacheHelper
    {
        public static object GetCachedData(string cacheKey)
        {
            var cachedData = MemoryCache.Default.Get(cacheKey);
            return cachedData;
        }

        public static void SetCachedData(string cacheKey, object cacheLock, int cacheTimePolicyMinutes, object dataToCache)
        {
            lock (cacheLock)
            {
                var cachedData = MemoryCache.Default.Get(cacheKey, null);

                if (cachedData != null)
                {
                    return;
                }

                var cip = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(cacheTimePolicyMinutes))
                };

                cachedData = dataToCache;
                MemoryCache.Default.Set(cacheKey, cachedData, cip);
            }
        }
    }
}