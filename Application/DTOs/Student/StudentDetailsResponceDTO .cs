namespace Application.DTOs.Student;

public class StudentDetailsResponseDTO
{
    public int? StudentId { get; set; }
    public string? Enrollment { get; set; }

    public string? SSOID { get; set; }

    public string? DateOfBirth { get; set; }
}
public class StudentEnrollmentResponseNewDTO
{
    public string? Enrollment { get; set; }
}
