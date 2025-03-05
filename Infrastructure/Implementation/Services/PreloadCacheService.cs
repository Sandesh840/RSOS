using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace Data.Implementation.Services;

public class PreloadCacheService : IHostedService
{
    private readonly IMemoryCache _cache;
    private readonly IWebHostEnvironment _env;

    public PreloadCacheService(IMemoryCache cache, IWebHostEnvironment env)
    {
        _cache = cache;
        _env = env;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var ebooksFolderPath = Path.Combine(_env.WebRootPath, "documents", "ebooks");
        var notificationsFolderPath = Path.Combine(_env.WebRootPath, "documents", "notifications");

        var ebookFiles = Directory.GetFiles(ebooksFolderPath);
        var notificationFiles = Directory.GetFiles(notificationsFolderPath);

        foreach (var file in ebookFiles)
        {
            var fileName = Path.GetFileName(file);
            
            var fileData = await File.ReadAllBytesAsync(file, cancellationToken);
            
            _cache.Set(fileName, fileData, TimeSpan.FromHours(100));
        }
        
        foreach (var file in notificationFiles)
        {
            var fileName = Path.GetFileName(file);
            
            var fileData = await File.ReadAllBytesAsync(file, cancellationToken);
            
            _cache.Set(fileName, fileData, TimeSpan.FromHours(100));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // _cache.Remove(key);
        
        return Task.CompletedTask;
    }
}