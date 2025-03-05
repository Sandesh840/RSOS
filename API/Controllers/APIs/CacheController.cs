using System.Net;
using Application.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Content;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Common.Utilities;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route("api/cache")]
[IgnoreAntiforgeryToken]
public class CacheControllerController : ControllerBase
{
    private readonly IMemoryCache _memoryCache;

    public CacheControllerController(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;

    }

    [Authorize]
    [HttpPost("clear-cache-bykey")]
    public IActionResult ClearCacheByKey(string cacheKey)
    {
        try
        {
            var _cacheUtility = new CacheUtility(_memoryCache);
            _cacheUtility.RemoveCacheByKey(cacheKey);

            var response = new ResponseDTO<List<ContentResponseDTO>>()
            {
                Status = "Success",
                Message = "Cache Removed Successfully",
                StatusCode = HttpStatusCode.OK,
            };
            return Ok(response);
        }
        catch (Exception ex) {
            var response = new ResponseDTO<List<ContentResponseDTO>>()
            {
                Status = "Failed",
                Message = ex.Message,
                StatusCode = HttpStatusCode.OK,
            };

            return Ok(response);
        }

    }

    [Authorize]
    [HttpPost("clear-cache-byprefix")]
    public IActionResult ClearCacheByPrefix(string prefix)
    {
        try
        {
            var _cacheUtility = new CacheUtility(_memoryCache);
            _cacheUtility.RemoveCacheByPrefix(prefix);

            var response = new ResponseDTO<List<ContentResponseDTO>>()
            {
                Status = "Success",
                Message = "Cache Removed Successfully",
                StatusCode = HttpStatusCode.OK,
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            var response = new ResponseDTO<List<ContentResponseDTO>>()
            {
                Status = "Failed",
                Message = ex.Message,
                StatusCode = HttpStatusCode.OK,
            };

            return Ok(response);
        }

    }

    [Authorize]
    [HttpPost("clear-all-cache")]
    public IActionResult ClearAllCache()
    {
        try
        {
            var _cacheUtility = new CacheUtility(_memoryCache);
            _cacheUtility.RemoveAllCache();

            var response = new ResponseDTO<List<ContentResponseDTO>>()
            {
                Status = "Success",
                Message = "Cache Removed Successfully",
                StatusCode = HttpStatusCode.OK,
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            var response = new ResponseDTO<List<ContentResponseDTO>>()
            {
                Status = "Failed",
                Message = ex.Message,
                StatusCode = HttpStatusCode.OK,
            };

            return Ok(response);
        }

    }


}