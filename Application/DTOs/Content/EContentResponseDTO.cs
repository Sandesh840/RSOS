namespace Application.DTOs.Content;

public class EContentResponseDTO
{
    public int Class { get; set; }
    
    public int SubjectId { get; set; }
    
    public int SubjectCode { get; set; }

    public string SubjectName { get; set; }
    
    public int ContentType { get; set; }

    public bool IsActive { get; set; }
    
    public List<Contents> ContentsList { get; set; }
}

public class Contents : ContentResponseDTO
{
    public int Sequence { get; set; }

    public int SubjectCode { get; set; }
    
    public int ContentType { get; set; }
    
    public bool IsActive { get; set; }
    
    public bool IsDeletable { get; set; }
    
    public string UploadedDate { get; set; }
}