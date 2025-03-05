namespace Application.DTOs.Content;

public class ContentChapterDTO
{
    public int Id { get; set; }
    
    public int Class { get; set; }
    
    public int SubjectId { get; set; }
    
    public string? SubjectName { get; set; }
    
     public string? ChapterName { get; set; }
   
}