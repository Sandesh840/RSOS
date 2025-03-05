using Application.DTOs.Base;
using Application.DTOs.Subject;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Application.DTOs.Question;
using Common.Constants;
using Microsoft.Extensions.Caching.Memory;
using static SkiaSharp.HarfBuzz.SKShaper;
using Application.DTOs.Tracking;
using Azure;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/subjects")]
[IgnoreAntiforgeryToken]
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;
    private readonly IMemoryCache _memoryCache;
    private readonly ICachePolicy _cachePolicy;

    public SubjectController(ISubjectService subjectService, IMemoryCache memoryCache, ICachePolicy cachePolicy)
    {
        _subjectService = subjectService;
        _memoryCache = memoryCache;
        _cachePolicy = cachePolicy;
    }

    [Authorize]
    [HttpPost("get-all-subjects")]
    public async Task<IActionResult> PostAllSubject([FromForm]SubjectRequestDTO subject)
    {
        var cacheKey = CacheName.SubjectMst.ToString() + subject.ClassId.ToString() ;
        var lstCacheData = new List<SubjectResponseDTO>();
        bool isExist = _memoryCache.TryGetValue(cacheKey, out lstCacheData);

        if (!isExist)
        {
            var result = await _subjectService.GetAllSubjects(subject.ClassId);

            var response = new ResponseDTO<List<SubjectResponseDTO>>
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
            var results = await _subjectService.GetAllSubjects(subject.ClassId);
            var response = new ResponseDTO<List<SubjectResponseDTO>>
            {
                Status = "Success",
                Message = "Successfully Retrieved",
                StatusCode = HttpStatusCode.OK,
                Result = lstCacheData ?? results
            };
            return Ok(response);
        }
        
        
    }



}

