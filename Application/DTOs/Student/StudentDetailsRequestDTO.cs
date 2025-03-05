namespace Application.DTOs.Student;

public class StudentDetailsRequestDTO
{
    public int Id { get; set; }

    public string? SSOID { get; set; }

    public string? DateOfBirth { get; set; }

    public string? Enrollment { get; set; }

    public int? StudentId { get; set; }

    public int? ClassId { get; set; }

    public int? SubjectId { get; set; }

    public int? MaximumMarks { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
}
