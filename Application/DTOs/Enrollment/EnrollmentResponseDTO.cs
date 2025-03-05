namespace Application.DTOs.Enrollment;

public class EnrollmentResponseDTO
{
    public int EnrollmentId { get; set; }
    
    public bool Status { get; set; }
    
    public string Reason { get; set; }
}