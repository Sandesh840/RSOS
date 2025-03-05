namespace Application.DTOs.Authentication;

public class AuthenticationResponseOldDTO
{
    public string? Type { get; set; }
    public LoginResponseEnrollmentDTO? loginResponseEnrollmentDTO { get; set; }
    public int Id { get; set; }
    
    public string Enrollment { get; set; }
    
    public string Name { get; set; }
    
    public string DateOfBirth { get; set; }
    
    public string SSOID { get; set; }
    
    public string ApplicationToken { get; set; }

    public string SecureRSOSToken { get; set; }

    public string ValidTill { get; set; }

    public string StartDate { get; set; }

    public string EndDate { get; set; }
    public string? Version { get; set; }
    
}