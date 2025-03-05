using System.Net;
using Application.DTOs.Authentication;
using Application.DTOs.Base;
using Application.DTOs.Question;
using Application.Interfaces.Services;
using Azure;
using Common.Constants;
using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/questions")]
[IgnoreAntiforgeryToken]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;
    private readonly IMemoryCache _memoryCache;
    private readonly ICachePolicy _cachePolicy;

    public QuestionController(IQuestionService questionService, IMemoryCache memoryCache, ICachePolicy cachePolicy)
    {
        _questionService = questionService;
        _memoryCache = memoryCache;
        _cachePolicy = cachePolicy;
    }
    
    [HttpGet("get-all-questions/{classId:int}/{subjectId:int}")]
    public async Task<IActionResult> GetAllQuestions(int classId, int subjectId)
    {
        var result = await _questionService.GetAllQuestions(classId, subjectId);

        var response = new ResponseDTO<QuestionResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }

    [HttpPost("get-all-questions")]
    public async Task<IActionResult> PostAllQuestions([FromForm] QuestionRequestDTO question)
    {
        var result = await _questionService.GetAllQuestions(question.ClassId, question.SubjectId);

        var response = new ResponseDTO<QuestionResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [Authorize]
    [HttpPost("get-all-questions-authorize")]
    public async Task<IActionResult> PostAllQuestionsAuthorize([FromForm] QuestionRequestDTO question)
    {
        var cacheKey = CacheName.QstMst.ToString() + question.ClassId.ToString() + question.SubjectId.ToString();
        var lstCacheData = new QuestionResponseDTO();
        bool isExist = _memoryCache.TryGetValue(cacheKey, out lstCacheData);
       
        if (!isExist) {
            var result = await _questionService.GetAllQuestions(question.ClassId, question.SubjectId);

            var response = new ResponseDTO<QuestionResponseDTO>
            {
                Status = "Success",
                Message = "Successfully Retrieved",
                StatusCode = HttpStatusCode.OK,
                Result = result
            };
            var cachePolicy = _cachePolicy.CacheExpiryOptions();
            _memoryCache.Set(cacheKey, result, cachePolicy[0]);
            return Ok(response);
        }
        else
        {
            var chkNull = new QuestionResponseDTO();
            var response = new ResponseDTO<QuestionResponseDTO>
            {
                Status = "Success",
                Message = "Successfully Retrieved",
                StatusCode = HttpStatusCode.OK,
                Result = lstCacheData ?? chkNull
            };           
            return Ok(response);
        }
       
    }


    [Authorize]
    [HttpPost("get-all-questions-authorize-new")]
    public async Task<IActionResult> PostAllQuestionsAuthorizeNew([FromForm] QuestionRequestDTO question)
    {
        var cacheKey = CacheName.QstMst.ToString() + question.ClassId.ToString() + question.SubjectId;
        byte[]? lstCacheData;
        bool isExist = _memoryCache.TryGetValue(cacheKey, out lstCacheData);

        if (!isExist)
        {
            var result = await _questionService.GetAllQuestions(question.ClassId, question.SubjectId);

            var response = new ResponseDTO<QuestionResponseDTO>
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
    [HttpPost("get-all-questions-authorize-by-topicid")]
    public async Task<IActionResult> PostAllQuestionsAuthorizeByTopicId([FromForm] QuestionRequestDTO question)
    {
        var cacheKey = CacheName.QstMstTopic.ToString() + question.ClassId.ToString() + question.SubjectId + question.TopicId;
        byte[]? lstCacheData;
        bool isExist = _memoryCache.TryGetValue(cacheKey, out lstCacheData);

        if (!isExist)
        {
            var result = await _questionService.GetAllQuestionsTopicWise(question.ClassId, question.SubjectId, question.TopicId);

            var response = new ResponseDTO<QuestionResponseDTO>
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

}