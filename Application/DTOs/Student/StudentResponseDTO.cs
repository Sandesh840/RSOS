namespace Application.DTOs.Student;

public class StudentResponseDTO
{
    public List<StudentResponsesResponseDTO> StudentResponses { get; set; } = null!;

    public List<StudentScoreResponseDTO> StudentScores { get; set; } = null!;
}

public class StudentResponsesResponseDTO
{
    public int Id { get; set; }
    
    public string? GUID { get; set; }

    public string? QuizGUID { get; set; }

    public int StudentId { get; set; }

    public int QuestionId { get; set; }

    public string? QuestionValue { get; set; }

    public int IsEdited { get; set; }

    public int IsUploaded { get; set; }
}

public class StudentScoreResponseDTO
{
    public int Id { get; set; }
    
    public string GUID { get; set; }
    
    public int StudentId { get; set; }

    public int Class { get; set; }

    public int SubjectId { get; set; }

    public int TopicId { get; set; }

    public string Score { get; set; } = null!;

    public int IsEdited { get; set; }

    public int IsUploaded { get; set; }
}

public class StudentResponseForHangfireDTO
{
    public string SSOID { get; set; } = null!;

    public string? DateOfBirth { get; set; }

    public int? StudentId { get; set; }
}