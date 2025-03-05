namespace Application.DTOs.Password;

public class ChangePasswordRequestDTO
{
    public string CurrentPassword { get; set;} = string.Empty;
    
    public string NewPassword { get; set; } = string.Empty;
    
    public string ConfirmNewPassword { get; set; } = string.Empty; 
    
    public string HdCurrentPassword { get; set; }
    
    public string HdNewPassword { get; set; }
    
    public string HdConfirmNewPassword { get; set; }
}