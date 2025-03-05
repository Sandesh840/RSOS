namespace Application.DTOs.Question;

public class QuestionRequestDTO
{
    public int? ClassId { get; set; }
    
    public int? SubjectId { get; set; }

    public int? TopicId { get; set; }
}