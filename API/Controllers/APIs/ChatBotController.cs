using System.Net;
using Application.DTOs.Base;
using Application.DTOs.ChatBot;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RSOS.Controllers.APIs;

[ApiController]
[IgnoreAntiforgeryToken]
[Route("api/chat-bot")]
public class ChatBotController : ControllerBase
{
    private readonly IChatBotService _chatBotService;

    public ChatBotController(IChatBotService chatBotService)
    {
        _chatBotService = chatBotService;
    }
    
    [Authorize]
    [HttpPost("get-chat-bot-messages")]
    public async Task<IActionResult> GetChatBotMessages()
    {
        var result = await _chatBotService.GetAllChatBotMessages();

        var response = new ResponseDTO<List<ChatBotResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}