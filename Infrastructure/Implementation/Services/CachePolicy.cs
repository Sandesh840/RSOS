using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Application.Interfaces.Services;

namespace Data.Implementation.Services
{

    public class CachePolicy : ICachePolicy
    {
        private readonly IMemoryCache _cache;
        public CachePolicy(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        public List<MemoryCacheEntryOptions> CacheExpiryOptions()
        {
            var cachePolicyOptionsList = new List<MemoryCacheEntryOptions>();
            DateTime endOfDay = DateTime.Today.AddDays(1).AddTicks(-1);
            TimeSpan timeRemaining = endOfDay - DateTime.Now;

            var cachePolicyLogin = new MemoryCacheEntryOptions
            {
                //AbsoluteExpiration = DateTime.Now.AddHours(24),
                AbsoluteExpiration = endOfDay,
                Priority = CacheItemPriority.High,
                //SlidingExpiration = TimeSpan.FromHours(24)
            };

            var cachePolicyAll = new MemoryCacheEntryOptions
            {
                //AbsoluteExpiration = DateTime.Now.AddHours(24),
                AbsoluteExpiration = endOfDay,
                Priority = CacheItemPriority.High,
                //SlidingExpiration = TimeSpan.FromHours(24)
            };

            cachePolicyOptionsList.Add(cachePolicyLogin);
            cachePolicyOptionsList.Add(cachePolicyAll);

            return cachePolicyOptionsList;
        }
       
    }
}
