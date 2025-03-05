using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Utilities
{
    public class CacheUtility
    {
        private readonly IMemoryCache _cache;

        public CacheUtility(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        #region Remove Cache By Key
        public void RemoveCacheByKey(string CacheKey)
        {
            _cache.Remove(CacheKey);
        }

        #endregion

        #region Remove Cache By Prefix
        public void RemoveCacheByPrefix(string Prefix)
        {
            var cacheEntries = GetCacheByPrefix(Prefix).ToList();
            foreach (var cacheEntry in cacheEntries)
            {
                _cache.Remove(cacheEntry.Key);
            }
        }

        public IEnumerable<KeyValuePair<string, object>> GetCacheByPrefix(string Prefix)
        {
            FieldInfo coherentStateField = typeof(MemoryCache).GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance);

            object coherentState = coherentStateField.GetValue(_cache);

            PropertyInfo entriesCollectionProperty = coherentState.GetType().GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);

            var entriesCollection = entriesCollectionProperty.GetValue(coherentState) as dynamic;


            if (entriesCollection != null)
            {
                foreach (var cacheItem in entriesCollection)
                {
                    if (cacheItem.GetType().IsGenericType && cacheItem.GetType().GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                    {
                        var key = cacheItem.GetType().GetProperty("Key").GetValue(cacheItem).ToString();
                        if (key.StartsWith(Prefix))
                        {
                            var value = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem);
                            yield return new KeyValuePair<string, object>(key, value);
                        }
                        //yield return new KeyValuePair<string, object>(cacheItem.Key.ToString(), cacheItem.Value);
                    }
                }
            }
        }

        #endregion

        #region Remove All Cache
        public void RemoveAllCache()
        {
            var cacheEntries = GetAllCache().ToList();
            foreach (var cacheEntry in cacheEntries)
            {
                _cache.Remove(cacheEntry.Key);
            }
        }

        public IEnumerable<KeyValuePair<string, object>> GetAllCache()
        {
            FieldInfo coherentStateField = typeof(MemoryCache).GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance);

            object coherentState = coherentStateField.GetValue(_cache);

            PropertyInfo entriesCollectionProperty = coherentState.GetType().GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);

            var entriesCollection = entriesCollectionProperty.GetValue(coherentState) as dynamic;


            if (entriesCollection != null)
            {
                foreach (var cacheItem in entriesCollection)
                {
                    if (cacheItem.GetType().IsGenericType && cacheItem.GetType().GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                    {
                        var key = cacheItem.GetType().GetProperty("Key").GetValue(cacheItem).ToString();
                        var value = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem);

                        yield return new KeyValuePair<string, object>(key, value);
                    }
                }
            }
        }

        #endregion

    }
}
