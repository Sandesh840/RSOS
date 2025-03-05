namespace Application.DTOs.ChatBot;

public class ChatBotResponseDTO
{
    public int MessageId { get; set; }
    
    public int MessageSubId { get; set; }
    
    public string Message { get; set; }
    
    public string MessageType { get; set; }
    
    public int Clickable { get; set; }
    
    public int LanguageId { get; set; }
}