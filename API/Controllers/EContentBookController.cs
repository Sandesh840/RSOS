using Application.DTOs.Dropdown;
using Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Application.DTOs.EContentBooks;
using Common.Constants;

namespace RSOS.Controllers;

[Authentication]
public class EContentBookController : BaseController<EContentBookController>
{
    private readonly IEBookService _eBookService;
    private readonly ISubjectService _subjectService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EContentBookController(IEBookService eBookService,ISubjectService subjectService, IWebHostEnvironment webHostEnvironment)
    {
        _eBookService = eBookService;
        _subjectService = subjectService;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public Task<IActionResult> Index()
    {
        return Task.FromResult<IActionResult>(View());
    }

    [HttpGet]
    public async Task<IActionResult> GetEbooksList(int classId, bool isActive)
    {       
        var result = await _eBookService.GetAllEBooks(classId, isActive);
        
        return Json(new
        {
            htmlData = ConvertViewToString("_EBookList", result, true)
        });
    }

    public async Task<IActionResult> DownloadEbook(string fileUrl)
    {
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        
        var filePath = Path.Combine(wwwRootPath, "documents", "ebooks", fileUrl);
        
        if (!System.IO.File.Exists(filePath)) return NotFound();
        
        byte[] fileContent;
        
        await using (var stream = new FileStream(filePath, FileMode.Open))
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);

                fileContent = memoryStream.ToArray();
            }
        }
        
        return File(fileContent, "application/pdf", fileUrl);
    }
    
    [HttpGet]
    public async Task<IActionResult> DeleteEBook(int eBookId)
    {
        await _eBookService.DeleteEBook(eBookId);

        return Json(new
        {
            data = "E-Book Successfully Deleted."
        });
    }

    [HttpPost]
    public async Task<IActionResult> SaveEBook(EContentBookRequestDTO eBookRequest)
    {
        var isValid = await _eBookService.UploadEContentBook(eBookRequest);

        if (isValid)
        {
            if (eBookRequest is { Id: 0, EBookFile: not null })
            {
                var eBookPath = DocumentUploadFilePath.EBooksDocumentFilePath;

                await UploadDocument(eBookPath, eBookRequest.EBookFile);
            }
            else
            {
                if (eBookRequest.EBookFile != null)
                {
                    var eBookPath = DocumentUploadFilePath.EBooksDocumentFilePath;

                    await UploadDocument(eBookPath, eBookRequest.EBookFile);
                }
            }
        }

        return Json(new
        {
            valid = isValid,
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetEBookById(int ebookId)
    {
        if (ebookId == 0)
        {
            return Json(new
            {
                data = ConvertViewToString("_AddUpdateEBook", new EContentBookRequestDTO()
                {
                    Id = 0
                }, true)
            });
        }
        
        var eBook = await _eBookService.GetEBookById(ebookId);

        return Json(new
        {
            data = ConvertViewToString("_AddUpdateEBook", eBook, true)
        });
    }
    
    [NonAction]
    private async Task UploadDocument(string folderPath, IFormFile file)
    {
        if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, folderPath)))
        {
            Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, folderPath));
        }

        var uploadedDocumentPath = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

        var fileName = $"{file.FileName}";

        await using var stream = new FileStream(Path.Combine(uploadedDocumentPath, fileName), FileMode.Create);

        await file.CopyToAsync(stream);
    }
}