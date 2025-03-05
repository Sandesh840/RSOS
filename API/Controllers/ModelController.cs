using Application.Interfaces.Services;
using Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RSOS.Controllers;

[Authentication]
public class ModelController : Controller
{
    private readonly IModelService _modelService;

    public ModelController(IModelService modelService)
    {
        _modelService = modelService;
    }

    public IActionResult Index()
    {
        var models = _modelService.GetAllDatabaseModels();
        
        ViewBag.ddlModels = new SelectList(models, "Id", "Value");
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ExportData(string model)
    {
        var result = await _modelService.ExportDatabaseModel(model);

        if (result.Item1)
        {
            return File(result.Item2, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{model}.xlsx");
        }

        return NotFound();
    }
}