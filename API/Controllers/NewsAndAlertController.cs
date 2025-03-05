using Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.NewsAndAlert;
using Application.Interfaces.Services;

namespace RSOS.Controllers;

public class NewsAndAlertController : BaseController<NewsAndAlertController>
{
    private readonly INewsAndAlertService _newsAndAlertService;

    public NewsAndAlertController(INewsAndAlertService newsAndAlertService)
    {
        _newsAndAlertService = newsAndAlertService;
    }

    [Authentication]
    public async Task<IActionResult> Index()
    {
        var result = await _newsAndAlertService.GetAllNewsAndAlert();

        return View(result);
    }

    [HttpGet]
    [Authentication]
    public async Task<IActionResult> GetNewsAndAlertById(int newsAndAlertId)
    {
        var newsAndAlert = await _newsAndAlertService.GetNewsAndAlertById(newsAndAlertId);

        return Json(new
        {
            data = newsAndAlert
        });
    }

    [HttpPost]
    [Authentication]
    public async Task<IActionResult> Upsert(NewsAndAlertRequestDTO newsAndAlert)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        newsAndAlert.UserId = userId ?? 1;

        if (string.IsNullOrEmpty(newsAndAlert.Description))
        {
            return Json(new
            {
                errorType = 1
            });
        }

        var action = 0;

        if (ExtensionMethods.IsMaliciousInput(newsAndAlert.Description))
        {
            return Json(new
            {
                errorType = -1,
                message = "The following description consists of malicious input, please try again."
            });
        }
        
        if (newsAndAlert.Id != 0)
        {
            action = 1;
            await _newsAndAlertService.UpdateNewsAndAlert(newsAndAlert);
        }
        else
        {
            action = 2;
            await _newsAndAlertService.InsertNewsAndAlert(newsAndAlert);
        }

        var result = await _newsAndAlertService.GetAllNewsAndAlert();

        return Json(new
        {
            action = action,
            htmlData = ConvertViewToString("_NewsAndAlertList", result, true)
        });
    }
    
    [HttpPost]
    [Authentication]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UpdateNewsAndAlertStatus(int newsAndAlertsId)
    {
        await _newsAndAlertService.UpdateNewsAndAlertStatus(newsAndAlertsId);

        var result = await _newsAndAlertService.GetAllNewsAndAlert();
        
        return Json(new
        {
            data = "News and Alert's status successfully changed.",
            htmlData = ConvertViewToString("_NewsAndAlertList", result, true)
        });
    }
}