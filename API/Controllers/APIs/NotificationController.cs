using System.Net;
using Application.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Notification;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/notifications")]
[IgnoreAntiforgeryToken]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

    public NotificationController(INotificationService notificationService, IWebHostEnvironment webHostEnvironment)
    {
        _notificationService = notificationService;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("get-valid-notifications")]
    public async Task<IActionResult> GetUserNotifications()
    {
        var result = await _notificationService.GetAllValidNotifications();
        
        var response = new ResponseDTO<List<NotificationResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    
    [HttpPost("get-valid-notifications")]
    public async Task<IActionResult> PostUserNotifications()
    {
        var result = await _notificationService.GetAllValidNotifications();

        var response = new ResponseDTO<List<NotificationResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [Authorize]
    [HttpPost("get-valid-notifications-authorize")]
    public async Task<IActionResult> PostUserNotificationsAuthorize()
    {
        var result = await _notificationService.GetAllValidNotifications();

        var response = new ResponseDTO<List<NotificationResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("send-notification")]
    public async Task<IActionResult> SendNotification(string registrationToken)
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
        
        await _notificationService.NotifyNotification(registrationToken);
        
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
    [HttpPost("send-notification-authorize")]
    public async Task<IActionResult> SendNotificationAuthorize(string registrationToken)
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

        await _notificationService.NotifyNotification(registrationToken);

        var response = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Notified.",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(response);
    }

    [HttpGet("download-notification-attachment/{notificationId}")]
    public async Task<IActionResult> DownloadNotificationAttachment(int notificationId)
    {
        var notification = await _notificationService.GetNotificationById(notificationId);

        var notFound = new ResponseDTO<object>()
        {
            Status = "Not Found",
            Message = "Attachment Not Found.",
            StatusCode = HttpStatusCode.NotFound,
            Result = false
        };
        
        if (string.IsNullOrEmpty(notification.UploadedFileUrl))
        {
            return NotFound(notFound);
        }
        
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        
        var filePath = Path.Combine(wwwRootPath, "documents", "notifications", notification.UploadedFileUrl);
        
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
        
            return File(memory, GetContentType(filePath), notification.UploadedFileName);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    [HttpPost("download-notification-attachment")]
    public async Task<IActionResult> PostDownloadNotificationAttachment(int? notificationId)
    {
        var notification = await _notificationService.GetNotificationById((int)notificationId!);

        var notFound = new ResponseDTO<object>()
        {
            Status = "Not Found",
            Message = "Attachment Not Found.",
            StatusCode = HttpStatusCode.NotFound,
            Result = false
        };
        
        if (string.IsNullOrEmpty(notification.UploadedFileUrl))
        {
            return NotFound(notFound);
        }
        
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        
        var filePath = Path.Combine(wwwRootPath, "documents", "notifications", notification.UploadedFileUrl);
        
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
        
            return File(memory, GetContentType(filePath), notification.UploadedFileName);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    [Authorize]
    [HttpPost("download-notification-attachment-authorize")]
    public async Task<IActionResult> PostDownloadNotificationAttachmentAuthorize([FromForm]int? notificationId)
    {
        var notification = await _notificationService.GetNotificationById((int)notificationId!);

        var notFound = new ResponseDTO<object>()
        {
            Status = "Not Found",
            Message = "Attachment Not Found.",
            StatusCode = HttpStatusCode.NotFound,
            Result = false
        };
        
        if (string.IsNullOrEmpty(notification.UploadedFileUrl))
        {
            return NotFound(notFound);
        }
        
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        
        var filePath = Path.Combine(wwwRootPath, "documents", "notifications", notification.UploadedFileUrl);
        
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
        
            return File(memory, GetContentType(filePath), notification.UploadedFileName);
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