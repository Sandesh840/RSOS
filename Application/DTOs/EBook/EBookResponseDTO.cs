namespace Application.DTOs.EBook;

public class EBookResponseDTO
{
    public int Id { get; set; }
    
    public int Code { get; set; }

    public int Class { get; set; }
    
    public string NameOfBook { get; set; }
    
    public string Volume { get; set; }
    
    public string FileName { get; set; }
}