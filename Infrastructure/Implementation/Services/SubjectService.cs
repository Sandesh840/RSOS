using Application.DTOs.Authentication;
using Application.DTOs.Student;
using Application.DTOs.Subject;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Common.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;



namespace Data.Implementation.Services;

public class SubjectService : ISubjectService
{
    private readonly IGenericRepository _genericRepository;

    public SubjectService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<SubjectResponseDTO> GetSubjectById(int subjectId)
    {
        var subject = await _genericRepository.GetByIdAsync<tblSubject>(subjectId);

        if (subject == null) return new SubjectResponseDTO();

        return new SubjectResponseDTO()
        {
            Id = subject.Id,
            SubjectCode = subject.SubjectCode,
            Title = subject.Title,
            TitleInHindi = subject.TitleInHindi,
            Class = subject.Class ?? 10
        };
    }

    public async Task<SubjectResponseDTO> GetSubjectByCode(int subjectCode)
    {
        var subject = await _genericRepository.GetFirstOrDefaultAsync<tblSubject>(x => x.SubjectCode == subjectCode);

        if (subject == null) return new SubjectResponseDTO();

        return new SubjectResponseDTO()
        {
            Id = subject.Id,
            SubjectCode = subject.SubjectCode,
            Title = subject.Title,
            TitleInHindi = subject.TitleInHindi,
            Class = subject.Class ?? 10
        };
    }
    
    public async Task<List<SubjectResponseDTO>> GetAllSubjects(int? @class)
    {
        var subjects =
            await _genericRepository.GetAsync<tblSubject>(x => 
                    (!@class.HasValue || x.Class == @class) && x.IsActive);

        return subjects.Select(x => new SubjectResponseDTO
        {
            Id = x.Id,
            Class = x.Class ?? 10,
            SubjectCode = x.SubjectCode,
            Title = x.Title,
            TitleInHindi = x.TitleInHindi
        }).ToList();
    }



}