using System.Text.Json.Serialization;

namespace Application.DTOs.Student;

public class StudentRequestDTO
{
    public string StudentResponse { get; set; }

    public string StudentScores { get; set; }
}

public class StudentTransactionRequestDTO
{
    public List<StudentResponseRequestDTO> StudentResponse { get; set; }

    public List<StudentScoreRequestDTO> StudentScores { get; set; }
}

public class StudentResponseRequestDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("guid")]
    public string? GUID { get; set; }

    [JsonPropertyName("quize_guid")]
    public string? QuizGUID { get; set; }

    [JsonPropertyName("student_id")]
    public int StudentId { get; set; }

    [JsonPropertyName("question_id")]
    public int QuestionId { get; set; }

    [JsonPropertyName("question_value")]
    public string? QuestionValue { get; set; }

    [JsonPropertyName("is_edited")]
    public int IsEdited { get; set; }

    [JsonPropertyName("is_uploaded")]
    public int IsUploaded { get; set; }

    [JsonPropertyName("is_deleted")]
    public int IsDeleted { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [JsonPropertyName("created_by")]
    public int CreatedBy { get; set; }
}

public class StudentScoreRequestDTO
{
    [JsonPropertyName("guid")]
    public string GUID { get; set; }
    
    [JsonPropertyName("student_id")]
    public int StudentId { get; set; }

    [JsonPropertyName("class_id")]
    public int Class { get; set; }

    [JsonPropertyName("subject_id")]
    public int SubjectId { get; set; }

    [JsonPropertyName("topic_id")]
    public int TopicId { get; set; }

    [JsonPropertyName("score")]
    public string? Score { get; set; }

    [JsonPropertyName("is_edited")]
    public int IsEdited { get; set; }

    [JsonPropertyName("is_uploaded")]
    public int IsUploaded { get; set; }
    
    [JsonPropertyName("is_deleted")]
    public int IsDeleted { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [JsonPropertyName("created_by")]
    public int CreatedBy { get; set; }
}