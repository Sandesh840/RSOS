using Application.DTOs.ChatBot;

namespace Application.Interfaces.Services;

public interface IChatBotService
{
    Task<List<ChatBotResponseDTO>> GetAllChatBotMessages();
}