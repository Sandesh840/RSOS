namespace Application.DTOs.Authentication;

public class RSOSLoginResponse
{
    public bool Status { get; set; }
    public string Type { get; set; }
}

public class LoginResponseDTO
{
    public string? Type { get; set; }
    public bool Status { get; set; }
    public ExtraDTO extra { get; set; }

    public StudentDataDTO Data { get; set; }
    
    public string secure_token { get; set; }
    
    public string secure_token_valid_till { get; set; }
    
    public List<object> Error { get; set; }
}

public class StudentDataDTO
{
    public StudentInfoDTO Student { get; set; }
}

public class StudentInfoDTO
{   public int? course { get; set; }
    public int? stream { get; set; }
    public string? Enrollment { get; set; }
    public string Name { get; set; }
    public DateTime Dob { get; set; }
    public string SsoId { get; set; }
    public string? student_status_at_different_level { get; set; }
    public int? exam_year { get; set; }
    public int? exam_month { get; set; }
    public int st_key { get; set; }
    public string? display { get; set; }

    public int Id { get; set; }
    //public int? course { get; set; }

    //public string? Enrollment { get; set; }

    //public string Name { get; set; }

    //public DateTime Dob { get; set; }

    //public string SsoId { get; set; }
    //public string? student_status_at_different_level { get; set; }
    //public int? exam_year { get; set; }
    //public int? exam_month { get; set; }
}
public class ExtraDTO
{
    public bool is_sessional_marks_entries_allowed { get; set; }
    public bool is_admit_card_allowed { get; set; }
}

public class EnrollmentResponse
{
    public string? type { get; set; }
    public bool Status { get; set; }
    public List<string> Data { get; set; }
    public string Error { get; set; }
}

public class LoginResponseEnrollmentDTO
{
    public string? type { get; set; }
    public bool? Status { get; set; }
    public EnrollementDataDTO? Data { get; set; }
    public List<object>? Error { get; set; }
}
public class EnrollementDataDTO
{
    public EnrollementListDTO? enrollementList { get; set; }
}
public class EnrollementListDTO
{
    public string? Enrollment { get; set; }
}
