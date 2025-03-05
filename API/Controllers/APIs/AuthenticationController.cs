using Application.DTOs.Authentication;
using Application.DTOs.Base;
using Application.Interfaces.Services;
using Azure;
using Common.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using Common.Utilities;
using Serilog;
using System.Diagnostics;
using Application.DTOs.Question;
using static SkiaSharp.HarfBuzz.SKShaper;
using static Google.Apis.Requests.BatchRequest;
using Data.Implementation.Services;
using RSOS.Helper;
using Microsoft.Extensions.Options;
using Application.DTOs.EBook;
using Application.DTOs.Authentication.Session;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/authentication")]
[IgnoreAntiforgeryToken]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMemoryCache _memoryCache;
    private readonly ICachePolicy _cachePolicy;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly LoginDictionary _loginDictionary;
    private readonly VersionSetting _versionSetting;

    public AuthenticationController(IAuthenticationService authenticationService, IMemoryCache memoryCache, ICachePolicy cachePolicy, ILogger<AuthenticationController> logger, IWebHostEnvironment webHostEnvironment,LoginDictionary loginDictionary, IOptions<VersionSetting> versionSetting)
    {
        _authenticationService = authenticationService;
        _memoryCache = memoryCache;
        _cachePolicy = cachePolicy;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
        _loginDictionary = loginDictionary;
        _versionSetting = versionSetting.Value;
    }

    [HttpPost("loginold")]
    public async Task<IActionResult> AuthenticateOld([FromForm] AuthenticationRequestDTO authenticationRequest)
    {
        //var stopwatch = Stopwatch.StartNew();

        var cacheKey = CacheName.Login.ToString() + authenticationRequest.SSOID.ToString() + authenticationRequest.DateOfBirth.ToString();
        var lstCacheData = new AuthenticationResponseDTO();
        bool isExist = _memoryCache.TryGetValue(cacheKey, out lstCacheData);

        //stopwatch.Stop();
        //var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        //_logger.LogError($"ExecutionTime :: {elapsedMilliseconds}");

        if (!isExist)
        {
            var response = await _authenticationService.Authenticate(authenticationRequest);

            switch (response.Id)
            {
                case 0:
                    {
                        var badRequest = new ResponseDTO<object>()
                        {
                            Status = "Bad Request",
                            Message = "Please insert a valid SSOID and Date of Birth.",
                            StatusCode = HttpStatusCode.BadRequest,
                            Result = false
                        };

                        var CacheUtility = new CacheUtility(_memoryCache);
                        CacheUtility.RemoveCacheByKey(cacheKey);

                        return BadRequest(badRequest);
                    }
                case -1:
                    {
                        var unauthorized = new ResponseDTO<object>()
                        {
                            Status = "Unauthorized",
                            Message = $"You account has been locked due to 5 attempts for invalid password, you can now try again after {response.ValidTill}",
                            StatusCode = HttpStatusCode.Unauthorized,
                            Result = false
                        };

                        var CacheUtility = new CacheUtility(_memoryCache);
                        CacheUtility.RemoveCacheByKey(cacheKey);

                        return Unauthorized(unauthorized);
                    }
                default:
                    {
                        var result = new ResponseDTO<AuthenticationResponseDTO>
                        {
                            StatusCode = HttpStatusCode.OK,
                            Result = response,
                            Message = "Successfully Logged In",
                            Status = "Success."
                        };

                        var CacheUtility = new CacheUtility(_memoryCache);
                        CacheUtility.RemoveCacheByKey(cacheKey);

                        var cachePolicy = _cachePolicy.CacheExpiryOptions();
                        _memoryCache.Set(cacheKey, response, cachePolicy[0]);

                        //var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, $"TextFile\\Token.txt");

                        //System.IO.File.WriteAllText(directoryPath, response.ApplicationToken);

                        return Ok(result);
                    }
            }
        }
        else
        {

            //var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, $"TextFile\\Token.txt");
            //string text = System.IO.File.ReadAllText(directoryPath);
            var chkNull = new AuthenticationResponseDTO();
            var result = new ResponseDTO<AuthenticationResponseDTO>
            {
                StatusCode = HttpStatusCode.OK,
                Result = lstCacheData ?? chkNull,
                //Result = text ,
                Message = "Successfully Logged In",
                Status = "Success."
            };

            return Ok(result);
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromForm] AuthenticationRequestDTO authenticationRequest)
    {
        var unauthorized = new ResponseDTO<object>()
        {
            Status = "Unauthorized",
            Message = $"ऐप का नया वर्जन({_versionSetting.VersionName}) उपलब्ध है। कृपया अपडेट करें।",
            StatusCode = HttpStatusCode.Unauthorized,
            Result = false
        };

        return Unauthorized(unauthorized);
    }


    [HttpPost("loginnew")]
    //[ServiceFilter(typeof(ActionNameLoggingFilter))]
    public async Task<IActionResult> AuthenticateNew([FromForm] AuthenticationRequestDTO authenticationRequest)
    {
        //_logger.LogError($"ExecutionStart :: {DateTime.UtcNow}");
        return Ok("Hello");       
    }


    [HttpPost("login-with-version")]
    public async Task<IActionResult> AuthenticateWithVersion([FromForm] AuthenticationRequestDTO authenticationRequest)
    {
        _logger.LogError($"Create cacheKey");
        var cacheKey = CacheName.Login.ToString() + authenticationRequest.SSOID.ToUpper().ToString() + authenticationRequest.DateOfBirth.ToString() + authenticationRequest.st_keys?.ToString() + authenticationRequest.VersionId;
        _logger.LogError($"CacheKey {cacheKey}");
        if (authenticationRequest.VersionId == _versionSetting.VersionName || authenticationRequest.VersionId == _versionSetting.VersionName2)
        {
            var lstCacheData = new AuthenticationResponseDTO();
            bool isExist = _memoryCache.TryGetValue(cacheKey, out lstCacheData);

            if (!isExist)
            {
                _logger.LogError($"CacheKey not exist {cacheKey}");
                var response = await _authenticationService.Authenticate(authenticationRequest);
                _logger.LogError($"Get response {response.Id}");
                switch (response.Id)
                {
                    case 0:
                        {
                            var badRequest = new ResponseDTO<object>()
                            {
                                Status = "Bad Request",
                                Message = "Please insert a valid SSOID and Date of Birth.",
                                StatusCode = HttpStatusCode.BadRequest,
                                Result = false
                            };

                            var CacheUtility = new CacheUtility(_memoryCache);
                            CacheUtility.RemoveCacheByKey(cacheKey);

                            return BadRequest(badRequest);
                        }
                    case -1:
                        {
                            var unauthorized = new ResponseDTO<object>()
                            {
                                Status = "Unauthorized",
                                Message = $"You account has been locked due to 5 attempts for invalid password, you can now try again after {response.ValidTill}",
                                StatusCode = HttpStatusCode.Unauthorized,
                                Result = false
                            };

                            var CacheUtility = new CacheUtility(_memoryCache);
                            CacheUtility.RemoveCacheByKey(cacheKey);

                            return Unauthorized(unauthorized);
                        }
                    case -2: // For Multiple Enrollement
                        {
                            var result = new ResponseDTO<object>()
                            {
                                StatusCode = HttpStatusCode.OK,
                                Status = "Success",
                                Message = "Multiple Enrollement",
                                LoginType = 2,
                                Result = response
                            };

                            var CacheUtility = new CacheUtility(_memoryCache);
                            CacheUtility.RemoveCacheByKey(cacheKey);

                            return Ok(result);
                        }
                    case -3: // For Exception
                        {
                            var result = new ResponseDTO<object>()
                            {
                                StatusCode = HttpStatusCode.BadRequest,
                                Status = "Success",
                                Message = response.Type??"",
                                LoginType = 2,
                                Result = response
                            };

                            var CacheUtility = new CacheUtility(_memoryCache);
                            CacheUtility.RemoveCacheByKey(cacheKey);

                            return Ok(result);
                        }
                    case -4: // For Exception
                        {
                            var result = new ResponseDTO<object>()
                            {
                                StatusCode = HttpStatusCode.BadRequest,
                                Status = "Success",
                                Message = response.Type ?? "",
                                LoginType = 2,
                                Result = response
                            };

                            var CacheUtility = new CacheUtility(_memoryCache);
                            CacheUtility.RemoveCacheByKey(cacheKey);

                            return Ok(result);
                        }
                    default:
                        {
                            var result = new ResponseDTO<AuthenticationResponseDTO>
                            {
                                StatusCode = HttpStatusCode.OK,
                                Status = "Success.",
                                Message = "Successfully Logged In",
                                LoginType = 1,
                                Result = response,
                            };

                            var CacheUtility = new CacheUtility(_memoryCache);
                            CacheUtility.RemoveCacheByKey(cacheKey);

                            var cachePolicy = _cachePolicy.CacheExpiryOptions();
                            _memoryCache.Set(cacheKey, response, cachePolicy[0]);


                            return Ok(result);
                        }
                }
            }
            else
            {
                var chkNull = new AuthenticationResponseDTO();
                var result = new ResponseDTO<AuthenticationResponseDTO>
                {
                    StatusCode = HttpStatusCode.OK,
                    Status = "Success.",
                    Message = "Successfully Logged In",
                    //LoginType = lstCacheData?.Enrollment != "" ? 1 : 2,
                    LoginType = lstCacheData?.Id == -2 ? 2 : 1,
                    Result = lstCacheData ?? chkNull
                };

                return Ok(result);
            }
           
        }
        else
        {
            var oldVersion = new ResponseDTO<object>()
            {
                Status = "Old Version",
                Message = $"ऐप का नया वर्जन({_versionSetting.VersionName}) उपलब्ध है। कृपया अपडेट करें।",
                StatusCode = HttpStatusCode.BadRequest,
                Result = false
            };

            var CacheUtility = new CacheUtility(_memoryCache);
            CacheUtility.RemoveCacheByKey(cacheKey);

            return BadRequest(oldVersion);
        }
    }


}