using System.Net;
using Application.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Content;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Common.Utilities;
using Newtonsoft.Json;
using Application.DTOs.Authentication;
using Common.Constants;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using static SkiaSharp.HarfBuzz.SKShaper;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.IO.Compression;


[ApiController]
[Route("api/contents")]
[IgnoreAntiforgeryToken]
public class ContentController : ControllerBase
{
    private readonly IContentService _contentService;
    private readonly IMemoryCache _memoryCache;
    private readonly ICachePolicy _cachePolicy;

    public ContentController(IContentService contentService, IMemoryCache memoryCache, ICachePolicy cachePolicy)
    {
        _contentService = contentService;
        _memoryCache = memoryCache;
        _cachePolicy = cachePolicy;
    }
    
    [HttpGet("get-content/{classId:int}/{subjectId:int}")]
    public async Task<IActionResult> GetContents(int classId, int subjectId)
    {
        var result = await _contentService.GetAllContents(classId, subjectId);

        var response = new ResponseDTO<List<ContentResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("get-content")]
    public async Task<IActionResult> PostContents([FromForm] ContentRequestDTO content)
    {
        var result = await _contentService.GetAllContents(content.ClassId, content.SubjectId);

        var response = new ResponseDTO<List<ContentResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    //[Authorize]
    //[HttpPost("get-content-authorize")]
    //public async Task<IActionResult> PostContentsAuthorize([FromForm] ContentRequestDTO content)
    //{
    //    var result = await _contentService.GetAllContents(content.ClassId, content.SubjectId);

    //    var response = new ResponseDTO<List<ContentResponseDTO>>()
    //    {
    //        Status = "Success",
    //        Message = "Successfully Retrieved",
    //        StatusCode = HttpStatusCode.OK,
    //        Result = result
    //    };

    //    return Ok(response);
    //}

    [Authorize]
    [HttpPost("get-content-authorize")]
    public IActionResult PostContentsAuthorize([FromForm] ContentRequestDTO content)
    {
        //var result = await _contentService.GetAllContents(content.ClassId, content.SubjectId);
        var result = new List<ContentResponseDTO>();
        var response = new ResponseDTO<List<ContentResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
 
    [Authorize]
    [HttpPost("get-content-authorize-new")]
    public async Task<IActionResult> PostContentsAuthorizeNew([FromForm] ContentRequestDTO content)
    {
        var cacheKey = CacheName.Content.ToString() + content.ClassId.ToString() + content.SubjectId;
        byte[] ?lstCacheData ;
        bool isExist = _memoryCache.TryGetValue(cacheKey, out lstCacheData);

        if (!isExist)
        {
            var result = await _contentService.GetAllContents(content.ClassId, content.SubjectId);

            var response = new ResponseDTO<List<ContentResponseDTO>>()
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

    [Authorize]
    [HttpPost("get-content-chapter")]
    public async Task<IActionResult> PostContentsChapterAuthorize([FromForm] ContentRequestDTO content)
    {
        var cacheKey = CacheName.ContentChpt.ToString() + content.ClassId.ToString() + content.SubjectId;
        byte[]? lstCacheData;
        bool isExist = _memoryCache.TryGetValue(cacheKey, out lstCacheData);

        if (!isExist)
        {
            var result = await _contentService.GetAllChapter(content.ClassId, content.SubjectId,"-");

            var response = new ResponseDTO<List<ContentChapterDTO>>()
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


    [Authorize]
    [HttpPost("get-content-authorize-new2")]
    public async Task<IActionResult> PostContentsAuthorizeNew2([FromForm] ContentRequestDTO content)
    {
        var result = await _contentService.GetAllContents(content.ClassId, content.SubjectId);
        //var result = new List<ContentResponseDTO>();
            var response = new ResponseDTO<List<ContentResponseDTO>>()
            {
                Status = "Success",
                Message = "Successfully Retrieved",
                StatusCode = HttpStatusCode.OK,
                Result = result
            };

            return Ok(response);
      
    }

}