using Application.DTOs.ChatBot;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class ChatBotService : IChatBotService
{
    private readonly IGenericRepository _genericRepository;

    public ChatBotService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<ChatBotResponseDTO>> GetAllChatBotMessages()
    {
        var chatBotMessages = await _genericRepository.GetAsync<tblChatBotMessage>(x => x.IsActive);

        var result = chatBotMessages.Select(x => new ChatBotResponseDTO()
        {
            MessageId = x.MessageId,
            Message = x.Message,
            Clickable = x.Clickable,
            LanguageId = (int)x.LanguageId!,
            MessageType = x.MessageType,
            MessageSubId = x.MessageSubId
        }).ToList();

        return result;
    }
}