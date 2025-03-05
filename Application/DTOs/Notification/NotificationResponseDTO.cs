namespace Application.DTOs.Notification;

public class NotificationResponseDTO
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? UploadedFileName { get; set; }

    public string? UploadedFileUrl { get; set; }

    public int IsActive { get; set; }

    public int IsTriggered { get; set; }

    public DateTime CreatedOn { get; set; }
    
    public DateTime ValidFrom { get; set; }

    public DateTime ValidTill { get; set; }
}