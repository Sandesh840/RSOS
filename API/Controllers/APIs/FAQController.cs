using System.Net;
using Application.DTOs.Base;
using Application.DTOs.FAQ;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/faqs")]
[IgnoreAntiforgeryToken]
public class FAQController : ControllerBase
{
    private readonly IFAQService _faqService;

    public FAQController(IFAQService faqService)
    {
        _faqService = faqService;
    }

    [HttpGet("get-all-faqs")]
    public async Task<IActionResult> GetAllFAQs()
    {
        var result = await _faqService.GetAllFAQs();

        var response = new ResponseDTO<List<FAQResponseDTO>>()
        {
            Status = "Success",
            Message = "FAQs Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    [HttpPost("get-all-faqs")]
    public async Task<IActionResult> PostAllFAQs()
    {
        var result = await _faqService.GetAllFAQs();

        var response = new ResponseDTO<List<FAQResponseDTO>>()
        {
            Status = "Success",
            Message = "FAQs Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("get-all-faqs-authorize")]
    public async Task<IActionResult> PostAllFAQsAuthorize()
    {
        var result = await _faqService.GetAllFAQs();

        var response = new ResponseDTO<List<FAQResponseDTO>>()
        {
            Status = "Success",
            Message = "FAQs Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}