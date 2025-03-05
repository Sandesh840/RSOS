namespace Application.DTOs.NewsAndAlert;

public class NewsAndAlertResponseDTO
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTill { get; set; }

    public int IsActive { get; set; }

    public DateTime CreatedOn { get; set; }
}