using Application.DTOs.Base;
using Application.DTOs.PcpDate;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RSOS.Controllers.APIs;

[ApiController]
[Route("api/pcp-dates")]
[IgnoreAntiforgeryToken]
public class PcpDatesController : ControllerBase
{
    private readonly IPcpDatesService _pcpDatesService;

    public PcpDatesController(IPcpDatesService pcpDatesService)
    {
        _pcpDatesService = pcpDatesService;
    }

    [HttpGet("get-pcp-Dates")]
    public async Task<IActionResult> GetAllPcpDates()
    {
        var result = await _pcpDatesService.GetAllPcpDates();

        var response = new ResponseDTO<List<PcpDatesResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}
