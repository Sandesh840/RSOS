namespace Application.DTOs.Enrollment;

public class EnrollmentRequestDTO
{
    public int EnrollmentId { get; set; }

    public int UserId { get; set; } 
    
    public bool Status { get; set; }
    
    public string Reason { get; set; }
}