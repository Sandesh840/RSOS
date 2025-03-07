using System.Net;
using Application.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Scheme;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/scheme")]
[IgnoreAntiforgeryToken]
public class SchemeController : ControllerBase
{
    private readonly ISchemeService _schemeService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

    public SchemeController(ISchemeService schemeService, IWebHostEnvironment webHostEnvironment)
    {
        _schemeService = schemeService;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("get-valid-schemes")]
    public async Task<IActionResult> GetUserSchemes()
    {
        var result = await _schemeService.GetAllValidSchemes();
        
        var response = new ResponseDTO<List<SchemeResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    
    [HttpPost("get-valid-schemes")]
    public async Task<IActionResult> PostUserSchemes()
    {
        var result = await _schemeService.GetAllValidSchemes();

        var response = new ResponseDTO<List<SchemeResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [Authorize]
    [HttpPost("get-valid-schemes-authorize")]
    public async Task<IActionResult> PostUserSchemesAuthorize()
    {
        var result = await _schemeService.GetAllValidSchemes();

        var response = new ResponseDTO<List<SchemeResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("send-scheme")]
    public async Task<IActionResult> SendScheme(string registrationToken)
    {
        if (string.IsNullOrEmpty(registrationToken))
        {
            var badRequest = new ResponseDTO<object>()
            {
                Status = "Bad Request",
                Message = "Invalid Request (missing device's registration token).",
                StatusCode = HttpStatusCode.BadRequest,
                Result = true
            };

            return BadRequest(badRequest);
        }
        
        await _schemeService.NotifyScheme(registrationToken);
        
        var response = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Notified.",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(response);
    }

    [Authorize]
    [HttpPost("send-scheme-authorize")]
    public async Task<IActionResult> SendSchemeAuthorize(string registrationToken)
    {
        if (string.IsNullOrEmpty(registrationToken))
        {
            var badRequest = new ResponseDTO<object>()
            {
                Status = "Bad Request",
                Message = "Invalid Request (missing device's registration token).",
                StatusCode = HttpStatusCode.BadRequest,
                Result = true
            };

            return BadRequest(badRequest);
        }

        await _schemeService.NotifyScheme(registrationToken);

        var response = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Notified.",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(response);
    }

    [HttpGet("download-scheme-attachment/{schemeId}")]
    public async Task<IActionResult> DownloadSchemeAttachment(int schemeId)
    {
        var scheme = await _schemeService.GetSchemeById(schemeId);

        var notFound = new ResponseDTO<object>()
        {
            Status = "Not Found",
            Message = "Attachment Not Found.",
            StatusCode = HttpStatusCode.NotFound,
            Result = false
        };

        if (scheme.SchemeFile == null || !scheme.SchemeFile.Any())
        {
            return NotFound(notFound);
        }

        var wwwRootPath = _webHostEnvironment.WebRootPath;

        var filePath = Path.Combine(wwwRootPath, "documents", "schemes", "scheme-" + scheme.Id, scheme.SchemeFile.First().FileUrl);

        await _semaphoreSlim.WaitAsync();

        try
        {
            if (!System.IO.File.Exists(filePath)) return NotFound(notFound);

            var memory = new MemoryStream();

            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            return File(memory, GetContentType(filePath), scheme.SchemeFile.First().FileUrl);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    [HttpPost("download-scheme-attachment")]
    public async Task<IActionResult> PostDownloadSchemeAttachment(int? schemeId)
    {
        var scheme = await _schemeService.GetSchemeById((int)schemeId!);

        var notFound = new ResponseDTO<object>()
        {
            Status = "Not Found",
            Message = "Attachment Not Found.",
            StatusCode = HttpStatusCode.NotFound,
            Result = false
        };


        if (scheme.SchemeFile == null || !scheme.SchemeFile.Any())
        {
            return NotFound(notFound);
        }

        //if (string.IsNullOrEmpty(scheme.UploadedFileUrl))
        //{
        //    return NotFound(notFound);
        //}

        var wwwRootPath = _webHostEnvironment.WebRootPath;

        //var filePath = Path.Combine(wwwRootPath, "documents", "schemes", scheme.UploadedFileUrl);
        var filePath = Path.Combine(wwwRootPath, "documents", "schemes", "scheme-" + scheme.Id, scheme.SchemeFile.First().FileUrl);

        await _semaphoreSlim.WaitAsync();

        try
        {
            if (!System.IO.File.Exists(filePath)) return NotFound(notFound);
            
            var memory = new MemoryStream();
        
            await using(var stream = new FileStream(filePath, FileMode.Open)) 
            {
                await stream.CopyToAsync(memory);
            }
        
            memory.Position = 0;
        
            return File(memory, GetContentType(filePath), scheme.SchemeFile.First().FileUrl);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    [Authorize]
    [HttpPost("download-scheme-attachment-authorize")]
    public async Task<IActionResult> PostDownloadSchemeAttachmentAuthorize([FromForm]int? schemeId)
    {
        var scheme = await _schemeService.GetSchemeById((int)schemeId!);

        var notFound = new ResponseDTO<object>()
        {
            Status = "Not Found",
            Message = "Attachment Not Found.",
            StatusCode = HttpStatusCode.NotFound,
            Result = false
        };

        //if (string.IsNullOrEmpty(scheme.UploadedFileUrl))
        //{
        //    return NotFound(notFound);
        //}

        if (scheme.SchemeFile == null || !scheme.SchemeFile.Any())
        {
            return NotFound(notFound);
        }

        var wwwRootPath = _webHostEnvironment.WebRootPath;

        // var filePath = Path.Combine(wwwRootPath, "documents", "schemes", scheme.UploadedFileUrl);
        var filePath = Path.Combine(wwwRootPath, "documents", "schemes", "scheme-" + scheme.Id, scheme.SchemeFile.First().FileUrl);

        await _semaphoreSlim.WaitAsync();

        try
        {
            if (!System.IO.File.Exists(filePath)) return NotFound(notFound);
            
            var memory = new MemoryStream();
        
            await using(var stream = new FileStream(filePath, FileMode.Open)) 
            {
                await stream.CopyToAsync(memory);
            }
        
            memory.Position = 0;
        
            return File(memory, GetContentType(filePath), scheme.SchemeFile.First().FileUrl);
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