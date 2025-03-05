namespace Application.DTOs.NewsAndAlert;

public class NewsAndAlertRequestDTO
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime ValidFrom { get; set; } = DateTime.Now;
    
    public DateTime ValidTill { get; set; }
}