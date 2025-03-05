namespace Application.DTOs.Student;

public class StudentExamDetailsResponseDTO
{
    public bool Status { get; set; }
    
    public StudentData Data { get; set; }
    
    public object Error { get; set; }
}

public class StudentData
{
    public StudentInfo Student { get; set; }
    
    public List<ExamSubject> Exam_Subjects { get; set; }
}

public class StudentInfo
{
    public int Id { get; set; }
    
    public string AiCode { get; set; }
    
    public string Enrollment { get; set; }
    
    public string Name { get; set; }
    
    public DateTime Dob { get; set; }
    
    public string SsoId { get; set; }
    
    public string FatherName { get; set; }
    
    public string MotherName { get; set; }
    
    public int GenderId { get; set; }
    
    public int Course { get; set; }
    
    public int IsEligible { get; set; }
    
    public string AiCenterName { get; set; }
}

public class ExamSubject
{
    public int Subject_Id { get; set; }
}