using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ICachePolicy
    {
        List<MemoryCacheEntryOptions> CacheExpiryOptions();
       
    }
}