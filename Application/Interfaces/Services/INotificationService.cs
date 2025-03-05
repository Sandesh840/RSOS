using Application.DTOs.Notification;

namespace Application.Interfaces.Services;

public interface INotificationService
{
    Task<List<NotificationResponseDTO>> GetAllNotifications();

    Task<List<NotificationResponseDTO>> GetAllValidNotifications();

    Task<NotificationResponseDTO> GetNotificationById(int notificationId);

    Task InsertNotification(NotificationRequestDTO notification);

    Task UpdateNotification(NotificationRequestDTO notification);

    Task UpdateNotificationStatus(int notificationId);

    Task NotifyNotification(int notificationId);
    
    Task NotifyNotification(string registrationToken);
}