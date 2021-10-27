using Microsoft.Extensions.Caching.Memory;
using System;

namespace PlayCore.Core.Extension
{
    public static class CacheExtensions
    {
        public static void Add(this IMemoryCache memoryCache, string cacheKey, object @object, MemoryCacheEntryOptions options)
        {
            memoryCache.Set(cacheKey, @object, options);
        }

        public static void Add(this IMemoryCache memoryCache, string cacheKey, object @object)
        {
            memoryCache.Set(cacheKey, @object);
        }

        public static void AddSlidingExp(this IMemoryCache memoryCache, string cacheKey, object @object, double minutes)
        {
            var cacheExpirationOptions =
                new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(minutes),
                    Priority = CacheItemPriority.Normal
                };

            Add(memoryCache, cacheKey, @object, cacheExpirationOptions);
        }

        public static void AddAbsoluteExp(this IMemoryCache memoryCache, string cacheKey, object @object, double minutes)
        {
            var cacheExpirationOptions =
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes),
                    Priority = CacheItemPriority.Normal
                };

            Add(memoryCache, cacheKey, @object, cacheExpirationOptions);
        }
    }
}
