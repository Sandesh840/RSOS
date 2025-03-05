namespace Application.DTOs.PCP;

public class PCPQuestionResponseDTO
{
    public int Id { get; set; }
    
    public int Code { get; set; }
    
    public string Subject { get; set; }
    
    public string Type { get; set; }
    
    public bool IsArchived { get; set; }
    
    public string UploadedDate { get; set; }
}