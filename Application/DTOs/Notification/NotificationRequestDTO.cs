using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Notification;

public class NotificationRequestDTO
{
    public int Id { get; set; }
    
    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public string? UploadedFileName { get; set; } = null!;

    public string? UploadedFileUrl { get; set; } = null!;

    public IFormFile? UploadedFile { get; set; } = null!;
    
    public DateTime ValidFrom { get; set; } = DateTime.Now;

    public DateTime ValidTill { get; set; }
}
