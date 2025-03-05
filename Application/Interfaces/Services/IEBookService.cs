using Application.DTOs.EBook;
using Application.DTOs.EContentBooks;

namespace Application.Interfaces.Services;

public interface IEBookService
{
    Task<List<EBookResponseDTO>> GetAllEBooks(int? classId, int? subjectCode, string? volume);
    
    Task<List<EContentBookResponseDTO>> GetAllEBooks(int classId, bool isActive);
    
    Task<bool> UploadEContentBook(EContentBookRequestDTO eBookRequest);

    Task<EContentBookRequestDTO> GetEBookById(int ebookId);

    Task DeleteEBook(int ebookId);
}