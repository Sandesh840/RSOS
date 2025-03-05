using Application.DTOs.Base;
using Application.DTOs.NewsAndAlert;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/news-and-alerts")]
[IgnoreAntiforgeryToken]
public class NewsAndAlertController : ControllerBase
{
    private readonly INewsAndAlertService _newsAndAlertService;
    
    public NewsAndAlertController(INewsAndAlertService newsAndAlertService)
    {
        _newsAndAlertService = newsAndAlertService;
    }

    [HttpGet("get-valid-news-and-alerts")]
    public async Task<IActionResult> GetUserNewsAndAlerts()
    {
        var result = await _newsAndAlertService.GetAllValidNewsAndAlert();

        var response = new ResponseDTO<List<NewsAndAlertResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("get-valid-news-and-alerts")]
    public async Task<IActionResult> PostUserNewsAndAlerts()
    {
        var result = await _newsAndAlertService.GetAllValidNewsAndAlert();

        var response = new ResponseDTO<List<NewsAndAlertResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [Authorize]
    [HttpPost("get-valid-news-and-alerts-authorize")]
    public async Task<IActionResult> PostUserNewsAndAlertsAuthorize()
    {
        var result = await _newsAndAlertService.GetAllValidNewsAndAlert();

        var response = new ResponseDTO<List<NewsAndAlertResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}
