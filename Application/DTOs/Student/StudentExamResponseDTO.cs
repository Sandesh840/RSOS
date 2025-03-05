namespace Application.DTOs.Student;

public class StudentExamResponseDTO
{
    public bool IsEligible { get; set; }
    
    public string Message { get; set; }
    
    public string PCPStartDate { get; set; }
    
    public string PCPEndDate { get; set; }
    
    public List<SubjectDetails> SubjectsList { get; set; }
}

public class SubjectDetails
{
    public int Id { get; set; }
    
    public int Code { get; set; }
    
    public int Class { get; set; }
    
    public string Name { get; set; }
}