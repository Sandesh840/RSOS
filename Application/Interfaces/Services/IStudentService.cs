using Application.DTOs.Student;

namespace Application.Interfaces.Services;

public interface IStudentService
{
    int StudentId { get; }

    string StudentDateOfBirth { get; }
    
    string StudentSSOID { get; }
    
    Task<StudentResponseDTO> GetStudentRecords(int studentId);

    Task<bool> InsertStudentResponse(List<StudentResponseRequestDTO> studentResponses);

    Task<bool> InsertStudentScore(List<StudentScoreRequestDTO> studentScores);

    Task<StudentExamResponseDTO> GetStudentExamSubjects(string secureToken, string enrollment);

    Task UploadStudentScores();
    Task<int> CheckIsScoreUploaded(int classId, int subjectId, int studentId);
    Task<IEnumerable<StudentDetailsResponseDTO>> GetStudentDetails();
    Task<int> InsertStudentData(StudentDetailsRequestDTO studentDetailsRequest);
    Task<bool> GetStudenEnrollment();
}