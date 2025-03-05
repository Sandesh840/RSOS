using Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace Data.Implementation.Services;

public class PDFService : IPdfService
{
    private readonly IMemoryCache _cache;
    private readonly IWebHostEnvironment _env;

    public PDFService(IMemoryCache cache, IWebHostEnvironment env)
    {
        _cache = cache;
        _env = env;
    }

    public IFileInfo GetCachedPdf(string fileName)
    {
        var cacheKey = $"pdf:{fileName}";
        
        if (!_cache.TryGetValue(cacheKey, out IFileInfo file))
        {
            var filePath = Path.Combine(_env.WebRootPath, "documents", "ebooks", fileName);

            file = new PhysicalFileInfo(new FileInfo(filePath));

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(file.Length);
            
            _cache.Set(cacheKey, file, cacheEntryOptions);
        }
        
        return file;
    }
}
