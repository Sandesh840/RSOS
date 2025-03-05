namespace Application.DTOs.Content;

public class EContentRequestDTO
{
    public int ClassId { get; set; }
    
    public int SubjectId { get; set; }
    
    public int ContentType { get; set; }     // All, Available (E-Content), Not-Available (Practice Paper)
    
    public bool IsActive { get; set; }
}