using System.Net;
using System.Text;
using Application.DTOs.Notification;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Data.Implementation.Services;

public class NotificationService : INotificationService
{
    private readonly IConfiguration _configuration;
    private readonly IGenericRepository _genericRepository;

    public NotificationService(IConfiguration configuration, IGenericRepository genericRepository)
    {
        _configuration = configuration;
        _genericRepository = genericRepository;
    }

    public async Task<List<NotificationResponseDTO>> GetAllNotifications()
    {
        var notifications = await _genericRepository.GetAsync<tblNotification>();
        
        return notifications.OrderByDescending(x => x.Id).Select(x => new NotificationResponseDTO
        {
            Id = x.Id,
            Title = x.Header,
            Description = x.Description,
            UploadedFileName = x.UploadedFileName,
            UploadedFileUrl = x.UploadedFileUrl,
            ValidFrom = x.ValidFrom,
            ValidTill = x.ValidTill,
            IsTriggered = x.IsTriggered ? 1 : 0,
            IsActive = x.IsActive ? 1 : 0,
            CreatedOn = x.CreatedOn
        }).ToList();
    }

    public async Task<List<NotificationResponseDTO>> GetAllValidNotifications()
    {
        var notifications = await _genericRepository.GetAsync<tblNotification>(x => 
            x.ValidFrom <= DateTime.Now && x.ValidTill >= DateTime.Now);
        
        return notifications.Select(x => new NotificationResponseDTO
        {
            Id = x.Id,
            Title = x.Header,
            Description = x.Description,
            UploadedFileName = x.UploadedFileName,
            UploadedFileUrl = x.UploadedFileUrl,
            ValidTill = x.ValidTill,
            ValidFrom = x.ValidFrom,
            IsActive = x.IsActive ? 1 : 0,
            IsTriggered = 1,
            CreatedOn = x.CreatedOn,
        }).ToList();
    }
    
    public async Task<NotificationResponseDTO> GetNotificationById(int notificationId)
    {
        var notification = await _genericRepository.GetByIdAsync<tblNotification>(notificationId);

        if (notification != null)
        {
            return new NotificationResponseDTO()
            {
                Id = notification.Id,
                Title = notification.Header,
                Description = notification.Description,
                ValidTill = notification.ValidTill,
                ValidFrom = notification.ValidFrom,
                UploadedFileUrl = notification.UploadedFileUrl,
                UploadedFileName = notification.UploadedFileName
            };
        }

        return new NotificationResponseDTO();
    }
    
    public async Task InsertNotification(NotificationRequestDTO notification)
    {
        var notificationModel = new tblNotification()
        {
            Header = notification.Title,
            Description = notification.Description,
            ValidFrom = notification.ValidFrom,
            ValidTill = notification.ValidTill,
            UploadedFileName = notification.UploadedFileName,
            UploadedFileUrl = notification.UploadedFileUrl,
            IsActive = true,
            CreatedBy = notification.UserId,
            CreatedOn = DateTime.Now
        };

        await _genericRepository.InsertAsync(notificationModel);
    }

    public async Task UpdateNotification(NotificationRequestDTO notification)
    {
        var notificationModel = await _genericRepository.GetByIdAsync<tblNotification>(notification.Id);

        if (notificationModel != null)
        {
            notificationModel.Header = notification.Title;
            notificationModel.Description = notification.Description;
            notificationModel.ValidTill = notification.ValidTill;
            notificationModel.ValidFrom = notification.ValidFrom;

            notificationModel.LastUpdatedBy = notification.UserId;
            notificationModel.LastUpdatedOn = DateTime.Now;
            
            if (notification.UploadedFileUrl != null)
            {
                notificationModel.UploadedFileName = notification.UploadedFileName;
                notificationModel.UploadedFileUrl = notification.UploadedFileUrl;
            }

            await _genericRepository.UpdateAsync(notificationModel);
        }
    }

    public async Task UpdateNotificationStatus(int notificationId)
    {
        var notificationModel = await _genericRepository.GetByIdAsync<tblNotification>(notificationId);
        
        if (notificationModel != null)
        {
            notificationModel.IsActive = !notificationModel.IsActive;
            
            await _genericRepository.UpdateAsync(notificationModel);
        }
    }

    public async Task NotifyNotification(int notificationId)
    {
        var notification = await _genericRepository.GetByIdAsync<tblNotification>(notificationId);

        if(notification == null) return;
        
        notification.IsTriggered = true;

        await _genericRepository.UpdateAsync(notification);
        
        var latestLoginDetails = (await _genericRepository
            .GetAsync<tblStudentLoginDetail>())
            .GroupBy(s => s.SSOID)
            .Select(g => g.OrderByDescending(s => s.LoginTime).First())
            .ToList();

        foreach (var detail in latestLoginDetails)
        {
            var userDeviceToken = detail.DeviceRegistrationToken;
            
            // Server Key from FCM Console
            var serverKey = $"key={_configuration["FCM:SERVER_KEY"]}";

            // Sender ID from FCM Console
            var senderId = $"id={_configuration["FCM:SENDER_ID"]}";

            var tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        
            tRequest.Method = "post";
        
            tRequest.Headers.Add($"Authorization: {serverKey}");
        
            tRequest.Headers.Add($"Sender: {senderId}");
        
            tRequest.ContentType = "application/json";
        
            var payload = new
            {
                to = userDeviceToken,
                notification = new
                {
                    title = notification.Header,
                    body = notification.Description
                },
                data = new 
                {
                    
                }
            };
        
            var postBody = JsonConvert.SerializeObject(payload);
        
            var byteArray = Encoding.UTF8.GetBytes(postBody);
        
            tRequest.ContentLength = byteArray.Length;

            await using var dataStream = await tRequest.GetRequestStreamAsync();
        
            await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
        
            using var tResponse = await tRequest.GetResponseAsync();
        
            await using var dataStreamResponse = tResponse.GetResponseStream();

            using var tReader = new StreamReader(dataStreamResponse);
        
            await tReader.ReadToEndAsync();
        }
    }
    
    public async Task NotifyNotification(string registrationToken)
    {
        // Server Key from FCM Console
        var serverKey = $"key={_configuration["FCM:SERVER_KEY"]}";

        // Sender ID from FCM Console
        var senderId = $"id={_configuration["FCM:SENDER_ID"]}";

        var tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
    
        tRequest.Method = "post";
    
        tRequest.Headers.Add($"Authorization: {serverKey}");
    
        tRequest.Headers.Add($"Sender: {senderId}");
    
        tRequest.ContentType = "application/json";
    
        var payload = new
        {
            to = registrationToken,
            notification = new
            {
                title = "Header",
                body = "Description"
            },
            data = new 
            {
                
            }
        };
    
        var postBody = JsonConvert.SerializeObject(payload);
    
        var byteArray = Encoding.UTF8.GetBytes(postBody);
    
        tRequest.ContentLength = byteArray.Length;

        await using var dataStream = await tRequest.GetRequestStreamAsync();
    
        await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
    
        using var tResponse = await tRequest.GetResponseAsync();
    
        await using var dataStreamResponse = tResponse.GetResponseStream();

        using var tReader = new StreamReader(dataStreamResponse);
    
        await tReader.ReadToEndAsync();
    }
}