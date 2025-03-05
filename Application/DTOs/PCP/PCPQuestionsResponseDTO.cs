namespace Application.DTOs.PCP;

public class PCPQuestionsResponseDTO
{
    public int ClassId { get; set; }

    public int PaperTypeId { get; set; }

    public string PaperType { get; set; }
    
    public List<Question> Questions { get; set; }
}

public class Question
{
    public int SubjectId { get; set; }

    public int SubjectCode { get; set; }
    
    public string SubjectName { get; set; }
    
    public int AttachmentId { get; set; }
    
    public string? PaperLastUploadedDate { get; set; }
}