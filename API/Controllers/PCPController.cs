using Application.DTOs.PCP;
using Application.Interfaces.Services;
using Common.Constants;
using Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Task = DocumentFormat.OpenXml.Office2021.DocumentTasks.Task;

namespace RSOS.Controllers;

[Authentication]
public class PCPController : BaseController<PCPController>
{
    private readonly ISubjectService _subjectService;
    private readonly IPCPService _pcpService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PCPController(IPCPService pcpService, IWebHostEnvironment webHostEnvironment, ISubjectService subjectService)
    {
        _pcpService = pcpService;
        _webHostEnvironment = webHostEnvironment;
        _subjectService = subjectService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetClassQuestions(int classId, int type)
    {
        var questions = await _pcpService.GetPCPQuestionsByClass(classId, type);
        
        return PartialView("_QuestionsList", questions);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadQuestionTemplate(int type)
    {
        var wwwRootPath = _webHostEnvironment.WebRootPath;

        var questionSheet = type == 1 ? "Practice Paper Questions Format Sheet.xlsx" : "ePCP Questions Format Sheet.xlsx";
        
        var filePath = Path.Combine(wwwRootPath, "documents", "templates", questionSheet);
        
        if (!System.IO.File.Exists(filePath)) return NotFound();
            
        var memory = new MemoryStream();
        
        await using(var stream = new FileStream(filePath, FileMode.Open)) 
        {
            await stream.CopyToAsync(memory);
        }
        
        memory.Position = 0;
        
        return File(memory, GetContentType(filePath), questionSheet);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetQuestionSheetDetails(int subjectId, int type)
    {
        var subject = await _subjectService.GetSubjectById(subjectId);

        var result = new PCPQuestionRequestDTO()
        {
            Code = subject.SubjectCode ?? 0,
            PaperTypeId = type,
            PaperType = type == 1 ? "Practice Paper" : "ePCP Final Paper",
            Class = subject.Class,
            Subject = subject.Title
        };
        
        return Json(new
        {
            htmlData = ConvertViewToString("_UploadQuestions", result, true)
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> UploadQuestionSheet(PCPQuestionRequestDTO pcpQuestion)
    {
        var result = await _pcpService.IsUploadedSheetValid(pcpQuestion);

        if (!result.Item1)
        {
            return Json(new
            {
                valid = 0,
                message = result.Item2
            });
        }
        
        var isUploaded = await _pcpService.UploadQuestions(pcpQuestion);

        if (!isUploaded)
        {
            return Json(new
            {
                valid = 0,
                message = "Please make sure the excel sheet is in correct format and a correct subject code is provided"
            });
        }
        
        var questionsSheetPath = DocumentUploadFilePath.QuestionsExcelDocumentFilePath;
            
        var serverDocName = await UploadDocument(questionsSheetPath, pcpQuestion.QuestionSheet);

        var questionSheet = new PCPQuestionSheetRequestDTO()
        {
            Class = pcpQuestion.Class,
            PaperType = pcpQuestion.PaperTypeId,
            SubjectId = (await _subjectService.GetSubjectByCode(pcpQuestion.Code)).Id,
            UploadedFileName = pcpQuestion.QuestionSheet.FileName,
            UploadedFileUrl = serverDocName
        };

        await _pcpService.UploadQuestionsWorksheet(questionSheet);

        var subjectQuestions = await _pcpService.GetPCPQuestionsByClass(pcpQuestion.Class, pcpQuestion.PaperTypeId);

        return Json(new
        {
            valid = 1,
            htmlData = ConvertViewToString("_QuestionsList", subjectQuestions, true),
            message = pcpQuestion.PaperTypeId == 1 ? "Practice Papers successfully uploaded" : "ePCP Question Papers successfully uploaded"
        });
    }
    
    [HttpGet]
    public async Task<IActionResult> DownloadQuestionSheet(int questionSheetId)
    {
        var questionSheet = await _pcpService.GetUploadedQuestionSheetById(questionSheetId);

        if (string.IsNullOrEmpty(questionSheet.Item1) ||
            string.IsNullOrEmpty(questionSheet.Item2))
            return Content($"File not found.");
        
        var filePath = DocumentUploadFilePath.QuestionsExcelDocumentFilePath;

        var sPhysicalPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath + questionSheet.Item2);

        return !System.IO.File.Exists(sPhysicalPath) ? Content($"File not found.") : DownloadAnyFile(questionSheet.Item1 ?? "", sPhysicalPath, null);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUploadedQuestionSheets(int subjectCode, int type)
    {
        var questionSheets = await _pcpService.GetUploadedQuestionSheets(subjectCode, type);

        return Json(new
        {
            htmlData = ConvertViewToString("_UploadedQuestionsList", questionSheets, true)
        });
    }

    [HttpPost]
    public async Task<JsonResult> ArchiveQuestionPaperSheet(int classId, int subjectId, int type)
    {
        await _pcpService.ArchiveQuestions(subjectId, type);
        
        var subjectQuestions = await _pcpService.GetPCPQuestionsByClass(classId, type);

        return Json(new
        {
            htmlData = ConvertViewToString("_QuestionsList", subjectQuestions, true),
            message = "The question papers of the following subject has been successfully archived.",
        });
    }
    
    [NonAction]
    private static string GetContentType(string path) 
    {
        var provider = new FileExtensionContentTypeProvider();
        
        if (!provider.TryGetContentType(path, out var contentType)) {
            contentType = "application/octet-stream";
        }
        
        return contentType;
    }
    
    [NonAction]
    private async Task<string> UploadDocument(string folderPath, IFormFile file)
    {
        if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, folderPath)))
        {
            Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, folderPath));
        }

        var uploadedDocumentPath = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);
        
        var extension = Path.GetExtension(file.FileName);
        
        var fileName = extension.SetUniqueFileName();

        await using var stream = new FileStream(Path.Combine(uploadedDocumentPath, fileName), FileMode.Create);
            
        await file.CopyToAsync(stream);
            
        return fileName;
    }
}