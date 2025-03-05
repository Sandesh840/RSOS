namespace Application.DTOs.Content;

public class ContentResponseDTO
{
    public int Id { get; set; }
    
    public int Class { get; set; }
    
    public int SubjectId { get; set; }
    
    public string SubjectName { get; set; }
    
    public string Faculty { get; set; }
    
    public string ChapterName { get; set; }
    
    public string Description { get; set; }
    
    public string PartName { get; set; }
    
    public int ChapterNo { get; set; }

    public int PartNo { get; set; }

    public int TimeInSeconds { get; set; }

    public string YouTubeLink { get; set; }
}