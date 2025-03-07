using Application.DTOs.Scheme;
using Application.DTOs.Scheme;
using Application.Interfaces.Services;
using Common.Constants;
using Common.Utilities;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using System.IO.Compression;

namespace RSOS.Controllers;

public class SchemeController : BaseController<SchemeController>
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ISchemeService _schemeService;

    public SchemeController(ISchemeService schemeService, IWebHostEnvironment webHostEnvironment)
    {
        _schemeService = schemeService;
        _webHostEnvironment = webHostEnvironment;
    }

    [Authentication]
    public async Task<IActionResult> Index()
    {
        var result = await _schemeService.GetAllSchemes();
        
        return View(result);
    }
    
    [HttpGet]
    [Authentication]
    public async Task<IActionResult> GetSchemeById(int schemeId)
    {
        var scheme = await _schemeService.GetSchemeById(schemeId);

        return Json(new
        {
            data = scheme
        });
    }

    [HttpPost]
    [Authentication]
    public async Task<IActionResult> Upsert(SchemeRequestDTO scheme, List<IFormFile>? files)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        
        scheme.UserId = userId ?? 1;

        if (string.IsNullOrEmpty(scheme.Title))
        {
            return Json(new
            {
                errorType = 1
            });
        }
        if (ExtensionMethods.IsMaliciousInput(scheme.Title) ||
            ExtensionMethods.IsMaliciousInput(scheme.Description))
        {
            return Json(new
            {
                errorType = -1,
                message = "The following heading title or description consists of malicious input, please try again."
            });
        }

        var action = 0;
        var message = "";       
        
        if(scheme.Id != 0)
        {
            action = 1;
            await _schemeService.UpdateScheme(scheme);

            if (files.Count>0)
            {
                var schemeFileData = await _schemeService.GetSchemeFileById(scheme.Id);

                foreach (var file in schemeFileData)
                {
                    if (!string.IsNullOrEmpty(file.FileUrl))
                    {
                        var oldImgPath = Path.Combine(_webHostEnvironment.WebRootPath, file.FileUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }
                    await _schemeService.DeleteSchemeFileById(file.Id);
                }
            }
        }
        else
        {
            action = 2;
            int newSchemeId=await _schemeService.InsertScheme(scheme);
            scheme.Id=newSchemeId;
        }

        string wwwRootPath = _webHostEnvironment.WebRootPath;
        if (files.Count > 0)
        {
            foreach (IFormFile file in files)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string schemePath = @"documents\schemes\scheme-" + scheme.Id;
                string finalPath = Path.Combine(wwwRootPath, schemePath);
                if (!Directory.Exists(finalPath)) { Directory.CreateDirectory(finalPath); }
                using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                tblSchemeFile productImage = new()
                {
                    FileUrl = @"\" + schemePath + @"\" + fileName,
                    FileId = scheme.Id,
                };
                if (scheme.tblSchemeFile == null)
                {
                    scheme.tblSchemeFile = new List<tblSchemeFile>();
                }
                scheme.tblSchemeFile.Add(productImage);
            }
            await _schemeService.UpdateScheme(scheme);

           // _unitOfWork.Product.Update(productVM.Product);
           // _unitOfWork.Save();
        }


        var result = await _schemeService.GetAllSchemes();

        return Json(new
        {
            action = action,
            message = message,
            htmlData = ConvertViewToString("_SchemeList", result, true)
        });
    }

    [HttpPost]
    [Authentication]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UpdateSchemeStatus(int schemeId)
    {
        await _schemeService.UpdateSchemeStatus(schemeId);

        var schemes = await _schemeService.GetAllSchemes();
        
        return Json(new
        {
            data = "Scheme's status successfully changed.",
            htmlData = ConvertViewToString("_SchemeList", schemes, true)
        });
    }
    
    [HttpPost]
    [Authentication]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> TriggerScheme(int schemeId)
    {
        await _schemeService.NotifyScheme(schemeId);

        var schemes = await _schemeService.GetAllSchemes();
        
        return Json(new
        {
            data = "Push scheme successfully triggered.",
            htmlData = ConvertViewToString("_SchemesList", schemes, true)
        });
    }

    //[Authentication]
    //public async Task<IActionResult> DownloadDocument(int schemeId)
    //{
    //    var scheme = await _schemeService.GetSchemeById(schemeId);

    //    if (scheme.Id == 0 || string.IsNullOrEmpty(scheme.UploadedFileName) ||
    //        string.IsNullOrEmpty(scheme.UploadedFileUrl))
    //        return Content($"File not found.");

    //    var filePath = DocumentUploadFilePath.SchemeDocumentFilePath;

    //    var sPhysicalPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath + scheme.UploadedFileUrl);

    //    return !System.IO.File.Exists(sPhysicalPath) ? Content($"file not found.") : DownloadAnyFile(scheme.UploadedFileName ?? "", sPhysicalPath, null);
    //}



    [Authentication]
    public async Task<IActionResult> DownloadDocument(int schemeId)
    {
        var scheme = await _schemeService.GetSchemeById(schemeId);

        // Check if the scheme exists and if any associated files are available
        if (scheme.Id == 0 || scheme.SchemeFile == null || !scheme.SchemeFile.Any())
            return Content($"File not found.");

        // Create a memory stream to store the zip file
        using (var memoryStream = new MemoryStream())
        {
            // Create a zip archive in the memory stream
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var schemeFile in scheme.SchemeFile)
                {
                    if (schemeFile != null && !string.IsNullOrEmpty(schemeFile.FileUrl))
                    {
                        // Get the physical file path
                        var sPhysicalPath = Path.Combine(_webHostEnvironment.WebRootPath, schemeFile.FileUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(sPhysicalPath))
                        {
                            // Create a zip entry for the file
                            var zipEntry = zipArchive.CreateEntry(schemeFile.FileUrl.Split('\\').Last());

                            // Write the file to the zip entry
                            using (var entryStream = zipEntry.Open())
                            using (var fileStream = new FileStream(sPhysicalPath, FileMode.Open, FileAccess.Read))
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }
                }
            }

            // Reset the memory stream position to the beginning
            memoryStream.Position = 0;

            // Return the zip file as a download
            return File(memoryStream.ToArray(), "application/zip", $"scheme_{schemeId}_files.zip");
        }
    }



    [Authentication]
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