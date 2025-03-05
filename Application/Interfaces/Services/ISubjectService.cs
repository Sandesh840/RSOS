using Application.DTOs.Subject;

namespace Application.Interfaces.Services;

public interface ISubjectService
{
    Task<SubjectResponseDTO> GetSubjectById(int subjectId);

    Task<SubjectResponseDTO> GetSubjectByCode(int subjectCode);

    Task<List<SubjectResponseDTO>> GetAllSubjects(int? @class);
}