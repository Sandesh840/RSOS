using Microsoft.AspNetCore.Http;

namespace Application.DTOs.EContentBooks;

public class EContentBookRequestDTO
{
    public int? Id { get; set; }
    
    public int? ClassId { get; set; }
    
    public int? SubjectId { get; set; }

    public string? SubjectName { get; set; }

    public string? NameOfBook { get; set; }

    public string? Volume { get; set; }
    
    public IFormFile? EBookFile { get; set; }
    
    public string? FileName { get; set; }
    
    public int? CreatedBy { get; set; }
}
