using Application.DTOs.Scheme;
using Model.Models;

namespace Application.Interfaces.Services;

public interface ISchemeService
{
    Task<List<SchemeResponseDTO>> GetAllSchemes();

    Task<List<SchemeResponseDTO>> GetAllValidSchemes();

    Task<SchemeResponseDTO> GetSchemeById(int schemeId);

    Task<List<tblSchemeFile>> GetSchemeFileById(int fileId);
    Task DeleteSchemeFileById(int fileId);
    Task<int> InsertScheme(SchemeRequestDTO scheme);

    Task UpdateScheme(SchemeRequestDTO scheme);

    Task UpdateSchemeStatus(int schemeId);

    Task NotifyScheme(int schemeId);
    
    Task NotifyScheme(string registrationToken);
}