using System.Net;
using Application.DTOs.Base;
using Application.DTOs.EBook;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using IPdfService = Application.Interfaces.Services.IPdfService;
using Application.DTOs.Content;
using Common.Constants;
using Common.Utilities;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/ebooks")]
[IgnoreAntiforgeryToken]
public class EBookController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly IPdfService _pdfService;
    private readonly IEBookService _eBookService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
    private readonly IMemoryCache _memoryCache;
    private readonly ICachePolicy _cachePolicy;

    public EBookController(IMemoryCache cache, IEBookService eBookService, IWebHostEnvironment webHostEnvironment, IPdfService pdfService,IMemoryCache memoryCache,ICachePolicy cachePolicy)
    {
        _cache = cache;
        _eBookService = eBookService;
        _webHostEnvironment = webHostEnvironment;
        _pdfService = pdfService;
        _memoryCache = memoryCache;
        _cachePolicy = cachePolicy;
    }

    [HttpGet("get-all-ebooks")]
    public async Task<IActionResult> GetAllEbooks(int? classId, int? subjectCode, string? volume)
    {
        var result = await _eBookService.GetAllEBooks(classId, subjectCode, volume);
        
        var response = new ResponseDTO<List<EBookResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("get-all-ebooks")]
    public async Task<IActionResult> PostAllEbooks([FromForm] EBookRequestDTO ebook)
    {
        var result = await _eBookService.GetAllEBooks(ebook.ClassId, ebook.SubjectId, ebook.Volume);

        var response = new ResponseDTO<List<EBookResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    //[Authorize]
    //[HttpPost("get-all-ebooks-authorize")]
    //public async Task<IActionResult> PostAllEbooksAuthorize([FromForm] EBookRequestDTO ebook)
    //{
    //    var result = await _eBookService.GetAllEBooks(ebook.ClassId, ebook.SubjectId, ebook.Volume);

    //    var response = new ResponseDTO<List<EBookResponseDTO>>()
    //    {
    //        Status = "Success",
    //        Message = "Successfully Retrieved",
    //        StatusCode = HttpStatusCode.OK,
    //        Result = result
    //    };

    //    return Ok(response);
    //}

    [Authorize]
    [HttpPost("get-all-ebooks-authorize")]
    public IActionResult PostAllEbooksAuthorize([FromForm] EBookRequestDTO ebook)
    {
        var result = new List<EBookResponseDTO>();
        var response = new ResponseDTO<List<EBookResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    //[Authorize]
    [HttpPost("get-all-ebooks-authorize-new")]
    public async Task<IActionResult> PostAllEbooksAuthorizeNew([FromForm] EBookRequestDTO ebook)
    {
        var cacheKey = CacheName.EBook.ToString() + ebook.ClassId.ToString() + ebook.SubjectId + ebook.Volume;
        byte[]? lstCacheData;
        bool isExist = _memoryCache.TryGetValue(cacheKey, out lstCacheData);

        if (!isExist)
        {
            var result = await _eBookService.GetAllEBooks(ebook.ClassId, ebook.SubjectId, ebook.Volume);

            var response = new ResponseDTO<List<EBookResponseDTO>>()
            {
                Status = "Success",
                Message = "Successfully Retrieved",
                StatusCode = HttpStatusCode.OK,
                Result = result
            }; 
            
            var _compressionUtility = new CompressionUtility();
            byte[] compressedBytes = _compressionUtility.CompressJson(response);

            var CacheUtility = new CacheUtility(_memoryCache);
            CacheUtility.RemoveCacheByKey(cacheKey);

            var cachePolicy = _cachePolicy.CacheExpiryOptions();
            _memoryCache.Set(cacheKey, compressedBytes, cachePolicy[0]);

            return File(compressedBytes, "application/gzip");
        }
        else
        {
            byte[] chkNull = new byte[1] { 123 };
            return File(lstCacheData ?? chkNull, "application/gzip");
        }    
    }

    [HttpGet("download-ebook/{fileUrl}")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> DownloadEbook(string fileUrl)
    {
        if (_cache.TryGetValue(fileUrl, out byte[]? cachedContent))
        {
            return File(cachedContent, "application/pdf", fileUrl);
        }
        
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        
        var filePath = Path.Combine(wwwRootPath, "documents", "ebooks", fileUrl);
        
        var notFound = new ResponseDTO<object>()
        {
            Status = "Not Found",
            Message = "EBook Not Found.",
            StatusCode = HttpStatusCode.NotFound,
            Result = false
        };

        await _semaphoreSlim.WaitAsync();
        
        try
        {
            if (!System.IO.File.Exists(filePath)) return NotFound(notFound);
            
            byte[] fileContent;
            
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);

                    fileContent = memoryStream.ToArray();
                }
            }
            
            return File(fileContent, "application/pdf", fileUrl);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    [HttpPost("download-ebook")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> PostDownloadEbook(string fileUrl)
    {
        if (_cache.TryGetValue(fileUrl, out byte[]? cachedContent))
        {
            return File(cachedContent, "application/pdf", fileUrl);
        }
        
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        
        var filePath = Path.Combine(wwwRootPath, "documents", "ebooks", fileUrl);
        
        var notFound = new ResponseDTO<object>()
        {
            Status = "Not Found",
            Message = "EBook Not Found.",
            StatusCode = HttpStatusCode.NotFound,
            Result = false
        };

        await _semaphoreSlim.WaitAsync();
        
        try
        {
            if (!System.IO.File.Exists(filePath)) return NotFound(notFound);
            
            byte[] fileContent;
            
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);

                    fileContent = memoryStream.ToArray();
                }
            }
            
            return File(fileContent, "application/pdf", fileUrl);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    [Authorize]
    [HttpPost("download-ebook-authorize")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> PostDownloadEbookAuthorize([FromForm]string fileUrl)
    {
        if (_cache.TryGetValue(fileUrl, out byte[]? cachedContent))
        {
            return File(cachedContent, "application/pdf", fileUrl);
        }
        
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        
        var filePath = Path.Combine(wwwRootPath, "documents", "ebooks", fileUrl);
        
        var notFound = new ResponseDTO<object>()
        {
            Status = "Not Found",
            Message = "EBook Not Found.",
            StatusCode = HttpStatusCode.NotFound,
            Result = false
        };

        await _semaphoreSlim.WaitAsync();
        
        try
        {
            if (!System.IO.File.Exists(filePath)) return NotFound(notFound);
            
            byte[] fileContent;
            
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);

                    fileContent = memoryStream.ToArray();
                }
            }
            
            return File(fileContent, "application/pdf", fileUrl);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private static string GetContentType(string path) {
        var provider = new FileExtensionContentTypeProvider();
        
        if (!provider.TryGetContentType(path, out var contentType)) {
            contentType = "application/octet-stream";
        }
        
        return contentType;
    }
}