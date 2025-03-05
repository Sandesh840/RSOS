namespace Application.DTOs.Question;

public class QuestionResponseDTO
{
    public List<Question> Questions { get; set; }

    public List<Common> Commons { get; set; }
}

public class Question
{
    public int Id { get; set; }

    public int QuestionTypeId { get; set; }

    public int Class { get; set; }

    public int SubjectId { get; set; }

    public int TopicId { get; set; }

    public int IsMandatory { get; set; }

    public int? Sequence { get; set; }

    public string? QuestionValue { get; set; }

    public int Flag { get; set; }
    
    public int PaperTypeId { get; set; }

    public string PaperType { get; set; }
}

public class Common
{
    public int Id { get; set; }

    public int Flag { get; set; }

    public int CommonId { get; set; }

    public string? Value { get; set; }

    public int? LanguageId { get; set; }

    public int CorrectAnswer { get; set; }

    public decimal? Score { get; set; }
}