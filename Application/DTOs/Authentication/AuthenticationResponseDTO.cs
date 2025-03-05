namespace Application.DTOs.Authentication;

public class AuthenticationResponseDTO
{
    public string? Type { get; set; }
    public LoginResponseEnrollmentDTO? EnrollmentList { get; set; }
    public int Id { get; set; }
    public int? course { get; set; }

    public string? Enrollment { get; set; }
    
    public string Name { get; set; }
    
    public string DateOfBirth { get; set; }
    
    public string SSOID { get; set; }
    public bool? is_sessional_marks_entries_allowed { get; set; }
    public bool? is_admit_card_allowed { get; set; }
    public string? student_status_at_different_level { get; set; }
    public int? exam_year { get; set; }
    public int? exam_month { get; set; }

    public string ApplicationToken { get; set; }

    public string SecureRSOSToken { get; set; }

    public string ValidTill { get; set; }

    public string StartDate { get; set; }

    public string EndDate { get; set; }
    public string? Version { get; set; }
    public List<KeyValuePair<int, string>>? displayData { get; set; }
    //public string? Type { get; set; }
    //public bool Status { get; set; }
    //public EnrollementDataDTO? Data { get; set; }

    public class LoginResponseEnrollmentDTO
    {
        public string? type { get; set; }
        public bool Status { get; set; }
        //public List<string>? Data { get; set; }

        public StDataDTO Data { get; set; }
        public string Error { get; set; }
    }

    public class StDataDTO
    {
        public List<int>? st_keys { get; set; }
        public Dictionary<int, string>? display { get; set; }
    }
}