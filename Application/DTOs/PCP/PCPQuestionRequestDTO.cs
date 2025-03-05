using Microsoft.AspNetCore.Http;

namespace Application.DTOs.PCP;

public class PCPQuestionRequestDTO
{
    public int Class { get; set; }
    
    public int Code { get; set; }
    
    public string Subject { get; set; }
    
    public int PaperTypeId { get; set; }
    
    public string PaperType { get; set; }
    
    public IFormFile QuestionSheet { get; set; }
}

public class PCPQuestionSheetRequestDTO
{
    public int PaperType { get; set; }
    
    public int Class { get; set; }
    
    public int SubjectId { get; set; }
    
    public string UploadedFileName { get; set; }
    
    public string UploadedFileUrl { get; set; }
}