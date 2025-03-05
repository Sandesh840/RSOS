using Application.DTOs.PcpDate;
using Application.Interfaces.Services;
using Common.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace RSOS.Controllers;

public class PcpDatesController : BaseController<PcpDatesController>
{
    private readonly IPcpDatesService _pcpDatesService;

    public PcpDatesController(IPcpDatesService pcpDatesService)
    {
        _pcpDatesService = pcpDatesService;
    }

    [Authentication]
    public async Task<IActionResult> Index()
    {
        var result = await _pcpDatesService.GetAllPcpDates();

        return View(result);
    }

    [HttpPost]
    [Authentication]
    public async Task<IActionResult> Insert(PcpDatesRequestDTO pcpDatesRequest)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        pcpDatesRequest.UserId = userId ?? 1;

        if (pcpDatesRequest.UserId != 0)
        {
            await _pcpDatesService.InsertPcpDates(pcpDatesRequest);
        }

        var result = await _pcpDatesService.GetAllPcpDates();

        return Json(new
        {
            htmlData = ConvertViewToString("_PcpDatesList", result, true)
        });
    }
    
    [HttpPost]
    [Authentication]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UpdatePcpDatesStatus(int pcpDatesId)
    {
        await _pcpDatesService.UpdatePcpDatesStatus(pcpDatesId);

        var result = await _pcpDatesService.GetAllPcpDates();
        
        return Json(new
        {
            data = "ePCP Date's status successfully changed.",
            htmlData = ConvertViewToString("_PcpDatesList", result, true)
        });
    }
}
