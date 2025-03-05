namespace Application.DTOs.Subject;

public class SubjectResponseDTO
{
    public int Id { get; set; }

    public int Class { get; set; }
    
    public int? SubjectCode { get; set; }
    
    public string Title { get; set; } = null!;

    public string? TitleInHindi { get; set; }
}