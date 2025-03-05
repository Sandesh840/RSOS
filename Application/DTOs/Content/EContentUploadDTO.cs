using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Content;

public class EContentUploadDTO
{
    public int SubjectCode { get; set; }
    
    public int ClassId { get; set; }
    
    public string Subject { get; set; }
    
    public IFormFile Contents { get; set; }
}